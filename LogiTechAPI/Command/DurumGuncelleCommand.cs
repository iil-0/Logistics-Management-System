using LogiTechAPI.Models;
using LogiTechAPI.Services;
using LogiTechAPI.State;
using Microsoft.Extensions.DependencyInjection;

namespace LogiTechAPI.Command
{
    public class DurumGuncelleCommand : IKargoCommand
    {
        // GonderiService yerine ScopeFactory tutuyoruz — Singleton Invoker
        // bizi DI ömür problemlerinden korur. Her Execute/Undo TAZE scope açar.
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
