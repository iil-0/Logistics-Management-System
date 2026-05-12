// Concrete Command — gönderiyi "Cancelled" durumuna alır.
// İş kuralı: yalnızca "Order Received" durumundaki kargolar iptal edilebilir.
// Undo: status "Order Received"a döner + "Cancelled" history satırı silinir.
using LogiTechAPI.Models;
using LogiTechAPI.Observer;
using LogiTechAPI.Services;
using LogiTechAPI.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LogiTechAPI.Command
{
    public class GonderiIptalCommand : IKargoCommand
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly string _trackingNo;
        private bool _executed;                          // Undo guard: Execute çağrılmadıysa Undo anlamsız
        private int? _addedHistoryId;                    // Silinecek "Cancelled" history satırı

        public string KomutAdi => "Cancel Shipment";

        public GonderiIptalCommand(IServiceScopeFactory scopeFactory, string trackingNo)
        {
            _scopeFactory = scopeFactory;
            _trackingNo = trackingNo;
        }

        public async Task<KomutSonuc> Execute()
        {
            using var scope = _scopeFactory.CreateScope();
            var gonderiService = scope.ServiceProvider.GetRequiredService<GonderiService>();
            var emailSettings = scope.ServiceProvider.GetRequiredService<IOptions<EmailSettings>>().Value;

            var shipment = await gonderiService.GetByTrackingNo(_trackingNo);
            if (shipment == null) return new KomutSonuc { Basarili = false, Mesaj = "Shipment not found!" };

            // İŞ KURALI — State pattern'in IsCancellable() davranışıyla aynı
            if (!string.Equals(shipment.Status, "Order Received", StringComparison.OrdinalIgnoreCase))
            {
                return new KomutSonuc { Basarili = false, Mesaj = $"Only shipments in 'Order Received' state can be cancelled. Current status is: {shipment.Status}" };
            }

            shipment.Status = "Cancelled";
            var history = new StatusHistory
            {
                Status = "Cancelled",
                Message = "The cargo shipment has been cancelled.",
                Date = DateTime.UtcNow
            };
            shipment.StatusHistory.Add(history);

            // OBSERVER — kullanıcıya iptal bildirimi gitsin
            gonderiService.RemoveObservers(_trackingNo);
            gonderiService.AddObserver(_trackingNo, new NotificationObserver());
            if (!string.IsNullOrWhiteSpace(shipment.SenderEmail))
            {
                gonderiService.AddObserver(_trackingNo, new EmailObserver(shipment.SenderEmail, _trackingNo, emailSettings));
            }
            gonderiService.NotifyObservers(_trackingNo, "Your shipment has been cancelled.", "Cancelled");

            await gonderiService.UpdateShipment(shipment);

            _addedHistoryId = history.Id;
            _executed = true;

            return new KomutSonuc { Basarili = true, Mesaj = "Shipment cancelled successfully." };
        }

        // Undo — iptali geri al: status "Order Received"a döner, "Cancelled" history silinir
        public async Task<KomutSonuc> Undo()
        {
            if (!_executed)
                return new KomutSonuc { Basarili = false, Mesaj = "Nothing to undo!" };

            using var scope = _scopeFactory.CreateScope();
            var gonderiService = scope.ServiceProvider.GetRequiredService<GonderiService>();

            var ok = await gonderiService.RevertStatus(_trackingNo, "Order Received", _addedHistoryId);
            if (!ok)
                return new KomutSonuc { Basarili = false, Mesaj = "Shipment not found!" };

            return new KomutSonuc { Basarili = true, Mesaj = "Cancellation undone!" };
        }
    }
}
