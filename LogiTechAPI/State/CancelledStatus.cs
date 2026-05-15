// Concrete State — alternatif TERMİNAL durum.
// Linear NextStatus akışına dahil değil; CancelShipmentCommand tarafından zorla atanır.
namespace LogiTechAPI.State
{
    public class CancelledStatus : IShipmentStatus
    {
        public string StatusName => "Cancelled";
        public string Description => "The cargo shipment has been cancelled.";
        public IShipmentStatus? NextStatus() => null;
        public bool IsCancellable() => false;
    }
}
