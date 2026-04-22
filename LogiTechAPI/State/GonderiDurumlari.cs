namespace LogiTechAPI.State
{
    /// <summary>
    /// State Pattern — Concrete State: Sipariş Alındı
    /// İlk durum. Sonraki: Hazırlanıyor. İptal edilebilir.
    /// </summary>
    public class SiparisAlindiDurum : IGonderiDurum
    {
        public string DurumAdi => "Order Received";
        public string Aciklama => "Your cargo order has been successfully registered.";
        public IGonderiDurum? SonrakiDurum() => new HazirlaniyorDurum();
        public bool IptalEdilabilir() => true;
    }

    public class HazirlaniyorDurum : IGonderiDurum
    {
        public string DurumAdi => "Preparing";
        public string Aciklama => "Your cargo is being prepared in the warehouse.";
        public IGonderiDurum? SonrakiDurum() => new YoldaDurum();
        public bool IptalEdilabilir() => false;
    }

    public class YoldaDurum : IGonderiDurum
    {
        public string DurumAdi => "On the Way";
        public string Aciklama => "Your cargo is on its way to the delivery address.";
        public IGonderiDurum? SonrakiDurum() => new TeslimEdildiDurum();
        public bool IptalEdilabilir() => true;
    }

    public class TeslimEdildiDurum : IGonderiDurum
    {
        public string DurumAdi => "Delivered";
        public string Aciklama => "Your cargo has been successfully delivered.";
        public IGonderiDurum? SonrakiDurum() => null;
        public bool IptalEdilabilir() => false;
    }

    public class IptalEdildiDurum : IGonderiDurum
    {
        public string DurumAdi => "Cancelled";
        public string Aciklama => "The cargo shipment has been cancelled.";
        public IGonderiDurum? SonrakiDurum() => null;
        public bool IptalEdilabilir() => false;
    }

    public static class GonderiDurumFactory
    {
        public static IGonderiDurum GetDurum(string durumAdi)
        {
            return durumAdi switch
            {
                "Order Received" or "Sipariş Alındı" => new SiparisAlindiDurum(),
                "Preparing" or "Hazırlanıyor"       => new HazirlaniyorDurum(),
                "On the Way" or "Yolda"             => new YoldaDurum(),
                "Delivered" or "Teslim Edildi"      => new TeslimEdildiDurum(),
                "Cancelled" or "İptal Edildi"       => new IptalEdildiDurum(),
                _                                   => new SiparisAlindiDurum()
            };
        }
    }
}
