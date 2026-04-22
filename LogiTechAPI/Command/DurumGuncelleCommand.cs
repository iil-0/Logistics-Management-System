using LogiTechAPI.Models;
using LogiTechAPI.Services;
using LogiTechAPI.State;

namespace LogiTechAPI.Command
{
    public class DurumGuncelleCommand : IKargoCommand
    {
        private readonly GonderiService _gonderiService;
        private readonly string _trackingNo;
        private readonly string _newStatus;
        private string? _oldStatus;

        public string KomutAdi => "Update Status";

        public DurumGuncelleCommand(GonderiService gonderiService, string trackingNo, string newStatus)
        {
            _gonderiService = gonderiService;
            _trackingNo = trackingNo;
            _newStatus = newStatus;
        }

        public async Task<KomutSonuc> Execute()
        {
            var shipment = await _gonderiService.GetByTrackingNo(_trackingNo);
            if (shipment == null) return new KomutSonuc { Basarili = false, Mesaj = "Shipment not found!" };

            _oldStatus = shipment.Status;
            
            // State Pattern
            IGonderiDurum statusObject = GonderiDurumFactory.GetStatus(_newStatus);
            
            shipment.Status = statusObject.StatusName;
            shipment.StatusHistory.Add(new StatusHistory 
            { 
                Status = statusObject.StatusName, 
                Message = statusObject.Description,
                Date = DateTime.Now
            });

            // Observer Pattern
            _gonderiService.NotifyObservers(_trackingNo, $"Status updated from {_oldStatus} to {statusObject.StatusName}", statusObject.StatusName);

            return new KomutSonuc { Basarili = true, Mesaj = $"Status successfully updated to {statusObject.StatusName}." };
        }

        public async Task<KomutSonuc> Undo()
        {
            if (_oldStatus != null)
            {
                var shipment = await _gonderiService.GetByTrackingNo(_trackingNo);
                if (shipment != null)
                {
                    shipment.Status = _oldStatus;
                    return new KomutSonuc { Basarili = true, Mesaj = "Status update undone!" };
                }
            }
            return new KomutSonuc { Basarili = false, Mesaj = "Nothing to undo!" };
        }
    }
}
