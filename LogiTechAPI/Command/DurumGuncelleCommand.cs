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
    public class DurumGuncelleCommand : IKargoCommand
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly string _trackingNo;
        private readonly string _newStatus;
        private string? _oldStatus;                       // Undo için: Execute öncesi durum
        private int? _addedHistoryId;                     // Undo için: silinecek history satırı

        public string KomutAdi => "Update Status";

        public DurumGuncelleCommand(IServiceScopeFactory scopeFactory, string trackingNo, string newStatus)
        {
            _scopeFactory = scopeFactory;
            _trackingNo = trackingNo;
            _newStatus = newStatus;
        }

        public async Task<KomutSonuc> Execute()
        {
            // Taze DI scope — DbContext fresh olsun
            using var scope = _scopeFactory.CreateScope();
            var gonderiService = scope.ServiceProvider.GetRequiredService<GonderiService>();
            var emailSettings = scope.ServiceProvider.GetRequiredService<IOptions<EmailSettings>>().Value;

            var shipment = await gonderiService.GetByTrackingNo(_trackingNo);
            if (shipment == null) return new KomutSonuc { Basarili = false, Mesaj = "Shipment not found!" };

            _oldStatus = shipment.Status;                 // Undo'da gerekecek

            // STATE — yeni durum nesnesini fabrikadan al
            IGonderiDurum statusObject = GonderiDurumFactory.GetStatus(_newStatus);

            shipment.Status = statusObject.StatusName;
            var newHistory = new StatusHistory
            {
                Status = statusObject.StatusName,
                Message = statusObject.Description,
                Date = DateTime.UtcNow
            };
            shipment.StatusHistory.Add(newHistory);

            // OBSERVER — kullanıcı için observer'ları taze kur (restart sonrası registry boş olabilir)
            gonderiService.RemoveObservers(_trackingNo);
            gonderiService.AddObserver(_trackingNo, new NotificationObserver());
            if (!string.IsNullOrWhiteSpace(shipment.SenderEmail))
            {
                gonderiService.AddObserver(_trackingNo, new EmailObserver(shipment.SenderEmail, _trackingNo, emailSettings));
            }

            // Subject → tüm observer'lara haber: log düşer, e-posta gider
            gonderiService.NotifyObservers(_trackingNo, $"Status updated from {_oldStatus} to {statusObject.StatusName}", statusObject.StatusName);

            await gonderiService.UpdateShipment(shipment);
            _addedHistoryId = newHistory.Id;              // SaveChanges sonrası ID dolar

            return new KomutSonuc { Basarili = true, Mesaj = $"Status successfully updated to {statusObject.StatusName}." };
        }

        // Undo — status'u eskiye çevir + history satırını sil (sanki olay hiç olmamış gibi)
        public async Task<KomutSonuc> Undo()
        {
            if (_oldStatus == null)
                return new KomutSonuc { Basarili = false, Mesaj = "Nothing to undo!" };

            using var scope = _scopeFactory.CreateScope();
            var gonderiService = scope.ServiceProvider.GetRequiredService<GonderiService>();

            // Tek transaction: hem status revert hem history delete
            var ok = await gonderiService.RevertStatus(_trackingNo, _oldStatus, _addedHistoryId);
            if (!ok)
                return new KomutSonuc { Basarili = false, Mesaj = "Shipment not found!" };

            return new KomutSonuc { Basarili = true, Mesaj = "Status update undone!" };
        }
    }
}
