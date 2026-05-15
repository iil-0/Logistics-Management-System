// Concrete Command — gönderinin durumunu (Status) bir adım ilerletir.
// State pattern fabrikası ile yeni durum nesnesi alır, Observer'larla bildirim
// gönderir. Undo: status'u eski değere döndürür + history satırını siler.
using LogiTechAPI.Models;
using LogiTechAPI.Observer;
using LogiTechAPI.Services;
using LogiTechAPI.Settings;
using LogiTechAPI.State;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LogiTechAPI.Command
{
    public class UpdateStatusCommand : ICargoCommand
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly string _trackingNo;
        private readonly string _newStatus;
        private string? _oldStatus;                       // Undo için: Execute öncesi durum
        private int? _addedHistoryId;                     // Undo için: silinecek history satırı

        public string CommandName => "Update Status";

        public UpdateStatusCommand(IServiceScopeFactory scopeFactory, string trackingNo, string newStatus)
        {
            _scopeFactory = scopeFactory;
            _trackingNo = trackingNo;
            _newStatus = newStatus;
        }

        public async Task<CommandResult> Execute()
        {
            // Taze DI scope — DbContext fresh olsun
            using var scope = _scopeFactory.CreateScope();
            var shipmentService = scope.ServiceProvider.GetRequiredService<ShipmentService>();
            var emailSettings = scope.ServiceProvider.GetRequiredService<IOptions<EmailSettings>>().Value;

            var shipment = await shipmentService.GetByTrackingNo(_trackingNo);
            if (shipment == null) return new CommandResult { Success = false, Message = "Shipment not found!" };

            _oldStatus = shipment.Status;                 // Undo'da gerekecek

            // STATE — yeni durum nesnesini fabrikadan al
            IShipmentStatus statusObject = ShipmentStatusFactory.GetStatus(_newStatus);

            shipment.Status = statusObject.StatusName;
            var newHistory = new StatusHistory
            {
                Status = statusObject.StatusName,
                Message = statusObject.Description,
                Date = DateTime.UtcNow
            };
            shipment.StatusHistory.Add(newHistory);

            // OBSERVER — kullanıcı için observer'ları taze kur (restart sonrası registry boş olabilir)
            shipmentService.RemoveObservers(_trackingNo);
            shipmentService.AddObserver(_trackingNo, new NotificationObserver());
            if (!string.IsNullOrWhiteSpace(shipment.SenderEmail))
            {
                shipmentService.AddObserver(_trackingNo, new EmailObserver(shipment.SenderEmail, _trackingNo, emailSettings));
            }

            // Subject → tüm observer'lara haber: log düşer, e-posta gider
            shipmentService.NotifyObservers(_trackingNo, $"Status updated from {_oldStatus} to {statusObject.StatusName}", statusObject.StatusName);

            await shipmentService.UpdateShipment(shipment);
            _addedHistoryId = newHistory.Id;              // SaveChanges sonrası ID dolar

            return new CommandResult { Success = true, Message = $"Status successfully updated to {statusObject.StatusName}." };
        }

        // Undo — status'u eskiye çevir + history satırını sil (sanki olay hiç olmamış gibi)
        public async Task<CommandResult> Undo()
        {
            if (_oldStatus == null)
                return new CommandResult { Success = false, Message = "Nothing to undo!" };

            using var scope = _scopeFactory.CreateScope();
            var shipmentService = scope.ServiceProvider.GetRequiredService<ShipmentService>();

            // Tek transaction: hem status revert hem history delete
            var ok = await shipmentService.RevertStatus(_trackingNo, _oldStatus, _addedHistoryId);
            if (!ok)
                return new CommandResult { Success = false, Message = "Shipment not found!" };

            return new CommandResult { Success = true, Message = "Status update undone!" };
        }
    }
}
