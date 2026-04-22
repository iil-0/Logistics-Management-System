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

            if (shipment.Status != "Order Received")
                return new KomutSonuc { Basarili = false, Mesaj = "Only shipments in 'Order Received' state can be cancelled." };

            _deletedShipment = shipment;
            await _gonderiService.DeleteShipment(_trackingNo);
            return new KomutSonuc { Basarili = true, Mesaj = "Shipment cancelled successfully." };
        }

        public async Task<KomutSonuc> Undo()
        {
            if (_deletedShipment != null)
            {
                await _gonderiService.AddShipment(_deletedShipment);
                return new KomutSonuc { Basarili = true, Mesaj = "Cancellation undone!" };
            }
            return new KomutSonuc { Basarili = false, Mesaj = "Nothing to undo!" };
        }
    }
}
