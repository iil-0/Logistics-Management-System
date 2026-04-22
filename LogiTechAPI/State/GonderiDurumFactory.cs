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
                _                                   => new OrderReceivedStatus()
            };
        }
    }
}
