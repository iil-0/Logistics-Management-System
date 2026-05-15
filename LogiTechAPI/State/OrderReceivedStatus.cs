// Concrete State — gönderinin BAŞLANGIÇ durumu.
// Tek iptal edilebilir durum (kargo henüz fiziksel olarak hareket etmedi).
namespace LogiTechAPI.State
{
    public class OrderReceivedStatus : IShipmentStatus
    {
        public string StatusName => "Order Received";
        public string Description => "Your cargo order has been successfully registered.";
        public IShipmentStatus? NextStatus() => new PreparingStatus();
        public bool IsCancellable() => true;
    }
}
