// Concrete State — kargo depoda paketlenip taşıyıcıya hazırlanıyor.
// Paketleme başladığı için iptal edilemez (iş kuralı).
namespace LogiTechAPI.State
{
    public class PreparingStatus : IShipmentStatus
    {
        public string StatusName => "Preparing";
        public string Description => "Your cargo is being prepared in the warehouse.";
        public IShipmentStatus? NextStatus() => new OnTheWayStatus();
        public bool IsCancellable() => false;
    }
}
