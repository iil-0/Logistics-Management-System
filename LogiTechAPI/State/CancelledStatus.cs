// Concrete State — alternatif TERMİNAL durum.
// Linear NextStatus akışına dahil değil; GonderiIptalCommand tarafından zorla atanır.
namespace LogiTechAPI.State
{
    public class CancelledStatus : IGonderiDurum
    {
        public string StatusName => "Cancelled";
        public string Description => "The cargo shipment has been cancelled.";
        public IGonderiDurum? NextStatus() => null;
        public bool IsCancellable() => false;
    }
}
