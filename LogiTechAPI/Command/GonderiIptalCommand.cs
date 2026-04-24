using LogiTechAPI.Models;
using LogiTechAPI.Services;

namespace LogiTechAPI.Command
{
    public class GonderiIptalCommand : IKargoCommand
    {
        private readonly GonderiService _gonderiService;
        private readonly string _trackingNo;
        private Gonderi? _deletedShipment;

        public string KomutAdi => "Cancel Shipment";

        public GonderiIptalCommand(GonderiService gonderiService, string trackingNo)
        {
            _gonderiService = gonderiService;
            _trackingNo = trackingNo;
        }

        public async Task<KomutSonuc> Execute()
        {
            var shipment = await _gonderiService.GetByTrackingNo(_trackingNo);
            if (shipment == null) return new KomutSonuc { Basarili = false, Mesaj = "Shipment not found!" };

            Console.WriteLine($"[DEBUG] Cancelling shipment {_trackingNo}. Current status in DB: '{shipment.Status}'");

            if (!string.Equals(shipment.Status, "Order Received", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"[DEBUG] Cancellation rejected. Status '{shipment.Status}' is not 'Order Received'.");
                return new KomutSonuc { Basarili = false, Mesaj = $"Only shipments in 'Order Received' state can be cancelled. Current status is: {shipment.Status}" };
            }

            _deletedShipment = shipment;
            
            var cancelStatus = LogiTechAPI.State.GonderiDurumFactory.GetStatus("Cancelled");
            shipment.Status = "Cancelled"; // Hardcoded to be safe
            
            var history = new StatusHistory 
            { 
                Status = "Cancelled", 
                Message = "The cargo shipment has been cancelled.",
                Date = DateTime.UtcNow
            };
            
            shipment.StatusHistory.Add(history);

            Console.WriteLine($"[DEBUG] Updating DB for shipment {_trackingNo} to 'Cancelled'...");
            await _gonderiService.UpdateShipment(shipment);
            Console.WriteLine($"[DEBUG] DB update completed for {_trackingNo}.");
            
            return new KomutSonuc { Basarili = true, Mesaj = "Shipment cancelled successfully." };
        }

        public async Task<KomutSonuc> Undo()
        {
            if (_deletedShipment != null)
            {
                var shipment = await _gonderiService.GetByTrackingNo(_trackingNo);
                if (shipment != null)
                {
                    shipment.Status = "Order Received";
                    await _gonderiService.UpdateShipment(shipment);
                    return new KomutSonuc { Basarili = true, Mesaj = "Cancellation undone!" };
                }
            }
            return new KomutSonuc { Basarili = false, Mesaj = "Nothing to undo!" };
        }
    }
}
