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
        private string? _oldStatus;
        private int? _addedHistoryId;

        public string KomutAdi => "Update Status";

        public DurumGuncelleCommand(IServiceScopeFactory scopeFactory, string trackingNo, string newStatus)
        {
            _scopeFactory = scopeFactory;
            _trackingNo = trackingNo;
            _newStatus = newStatus;
        }

        public async Task<KomutSonuc> Execute()
        {
            using var scope = _scopeFactory.CreateScope();
            var gonderiService = scope.ServiceProvider.GetRequiredService<GonderiService>();
            var emailSettings = scope.ServiceProvider.GetRequiredService<IOptions<EmailSettings>>().Value;

            var shipment = await gonderiService.GetByTrackingNo(_trackingNo);
            if (shipment == null) return new KomutSonuc { Basarili = false, Mesaj = "Shipment not found!" };

            _oldStatus = shipment.Status;

            IGonderiDurum statusObject = GonderiDurumFactory.GetStatus(_newStatus);

            shipment.Status = statusObject.StatusName;
            var newHistory = new StatusHistory
            {
                Status = statusObject.StatusName,
                Message = statusObject.Description,
                Date = DateTime.UtcNow
            };
            shipment.StatusHistory.Add(newHistory);

            // Observer'ları taze kaydet — backend restart sonrası Singleton
            // ObserverRegistry boşalmış olabilir, bu yüzden her status değişiminde
            // observer listesini gönderici bilgisinden yeniden inşa ediyoruz.
            gonderiService.RemoveObservers(_trackingNo);
            gonderiService.AddObserver(_trackingNo, new NotificationObserver());
            if (!string.IsNullOrWhiteSpace(shipment.SenderEmail))
            {
                gonderiService.AddObserver(_trackingNo, new EmailObserver(shipment.SenderEmail, _trackingNo, emailSettings));
            }

            gonderiService.NotifyObservers(_trackingNo, $"Status updated from {_oldStatus} to {statusObject.StatusName}", statusObject.StatusName);

            await gonderiService.UpdateShipment(shipment);

            _addedHistoryId = newHistory.Id;

            return new KomutSonuc { Basarili = true, Mesaj = $"Status successfully updated to {statusObject.StatusName}." };
        }

        public async Task<KomutSonuc> Undo()
        {
            if (_oldStatus == null)
                return new KomutSonuc { Basarili = false, Mesaj = "Nothing to undo!" };

            using var scope = _scopeFactory.CreateScope();
            var gonderiService = scope.ServiceProvider.GetRequiredService<GonderiService>();

            var ok = await gonderiService.RevertStatus(_trackingNo, _oldStatus, _addedHistoryId);
            if (!ok)
                return new KomutSonuc { Basarili = false, Mesaj = "Shipment not found!" };

            return new KomutSonuc { Basarili = true, Mesaj = "Status update undone!" };
        }
    }
}
