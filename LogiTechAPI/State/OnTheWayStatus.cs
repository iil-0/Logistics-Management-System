// Concrete State — kargo araçta, alıcı adresine doğru hareket halinde.
namespace LogiTechAPI.State
{
    public class OnTheWayStatus : IShipmentStatus
    {
        public string StatusName => "On the Way";
        public string Description => "Your cargo is on its way to the delivery address.";
        public IShipmentStatus? NextStatus() => new DeliveredStatus();
        public bool IsCancellable() => false;
    }
}
