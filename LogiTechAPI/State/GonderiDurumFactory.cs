// DB ile State pattern arasındaki köprü. DB'de Status string olarak saklanır;
// bu factory string'i alıp uygun IGonderiDurum nesnesine çevirir.
// Command'lar (DurumGuncelle, GonderiIptal) ve Controller (IsCancellable flag'i) kullanır.
namespace LogiTechAPI.State
{
    public static class GonderiDurumFactory
    {
        public static IGonderiDurum GetStatus(string statusName)
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
