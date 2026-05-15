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
    public class CancelShipmentCommand : ICargoCommand
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly string _trackingNo;
        private bool _executed;                          // Undo guard: Execute çağrılmadıysa Undo anlamsız
        private int? _addedHistoryId;                    // Silinecek "Cancelled" history satırı

        public string CommandName => "Cancel Shipment";

        public CancelShipmentCommand(IServiceScopeFactory scopeFactory, string trackingNo)
        {
            _scopeFactory = scopeFactory;
            _trackingNo = trackingNo;
        }

        public async Task<CommandResult> Execute()
        {
            using var scope = _scopeFactory.CreateScope();
            var shipmentService = scope.ServiceProvider.GetRequiredService<ShipmentService>();
            var emailSettings = scope.ServiceProvider.GetRequiredService<IOptions<EmailSettings>>().Value;

            var shipment = await shipmentService.GetByTrackingNo(_trackingNo);
            if (shipment == null) return new CommandResult { Success = false, Message = "Shipment not found!" };

            // İŞ KURALI — State pattern'in IsCancellable() davranışıyla aynı
            if (!string.Equals(shipment.Status, "Order Received", StringComparison.OrdinalIgnoreCase))
            {
                return new CommandResult { Success = false, Message = $"Only shipments in 'Order Received' state can be cancelled. Current status is: {shipment.Status}" };
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
            shipmentService.RemoveObservers(_trackingNo);
            shipmentService.AddObserver(_trackingNo, new NotificationObserver());
            if (!string.IsNullOrWhiteSpace(shipment.SenderEmail))
            {
                shipmentService.AddObserver(_trackingNo, new EmailObserver(shipment.SenderEmail, _trackingNo, emailSettings));
            }
            shipmentService.NotifyObservers(_trackingNo, "Your shipment has been cancelled.", "Cancelled");

            await shipmentService.UpdateShipment(shipment);

            _addedHistoryId = history.Id;
            _executed = true;

            return new CommandResult { Success = true, Message = "Shipment cancelled successfully." };
        }

        // Undo — iptali geri al: status "Order Received"a döner, "Cancelled" history silinir
        public async Task<CommandResult> Undo()
        {
            if (!_executed)
                return new CommandResult { Success = false, Message = "Nothing to undo!" };

            using var scope = _scopeFactory.CreateScope();
            var shipmentService = scope.ServiceProvider.GetRequiredService<ShipmentService>();

            var ok = await shipmentService.RevertStatus(_trackingNo, "Order Received", _addedHistoryId);
            if (!ok)
                return new CommandResult { Success = false, Message = "Shipment not found!" };

            return new CommandResult { Success = true, Message = "Cancellation undone!" };
        }
    }
}
