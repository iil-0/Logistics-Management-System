// DB ile State pattern arasındaki köprü. DB'de Status string olarak saklanır;
// bu factory string'i alıp uygun IShipmentStatus nesnesine çevirir.
// Command'lar (UpdateStatus, CancelShipment) ve Controller (IsCancellable flag'i) kullanır.
namespace LogiTechAPI.State
{
    public static class ShipmentStatusFactory
    {
        public static IShipmentStatus GetStatus(string statusName)
        {
            return statusName?.ToLower() switch
            {
                "order received" or "sipariş alındı" => new OrderReceivedStatus(),
                "preparing" or "hazırlanıyor"       => new PreparingStatus(),
                "on the way" or "yolda"             => new OnTheWayStatus(),
                "delivered" or "teslim edildi"      => new DeliveredStatus(),
                "cancelled" or "iptal edildi"       => new CancelledStatus(),
                _                                   => new OrderReceivedStatus()  // Veri bozulmasına karşı güvenli varsayılan
            };
        }
    }
}
