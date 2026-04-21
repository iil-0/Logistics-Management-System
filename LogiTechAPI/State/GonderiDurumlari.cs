namespace LogiTechAPI.State
{
    /// <summary>
    /// State Pattern — Concrete State: Sipariş Alındı
    /// İlk durum. Sonraki: Hazırlanıyor. İptal edilebilir.
    /// </summary>
    public class SiparisAlindiDurum : IGonderiDurum
    {
        public string DurumAdi => "Sipariş Alındı";
        public string Aciklama => "Kargo siparişiniz sisteme başarıyla kaydedildi.";
        public IGonderiDurum? SonrakiDurum() => new HazirlaniyorDurum();
        public bool IptalEdilabilir() => true;
    }

    /// <summary>
    /// State Pattern — Concrete State: Hazırlanıyor
    /// Paket hazırlanıyor. Sonraki: Yolda. İptal edilemez.
    /// </summary>
    public class HazirlaniyorDurum : IGonderiDurum
    {
        public string DurumAdi => "Hazırlanıyor";
        public string Aciklama => "Kargonuz depoda hazırlanıyor.";
        public IGonderiDurum? SonrakiDurum() => new YoldaDurum();
        public bool IptalEdilabilir() => false;
    }

    /// <summary>
    /// State Pattern — Concrete State: Yolda
    /// Kargo taşınıyor. Sonraki: Teslim Edildi. İptal edilebilir.
    /// </summary>
    public class YoldaDurum : IGonderiDurum
    {
        public string DurumAdi => "Yolda";
        public string Aciklama => "Kargonuz teslim adresine doğru yola çıktı.";
        public IGonderiDurum? SonrakiDurum() => new TeslimEdildiDurum();
        public bool IptalEdilabilir() => true;
    }

    /// <summary>
    /// State Pattern — Concrete State: Teslim Edildi
    /// Son durum. Geçiş yok. İptal edilemez.
    /// </summary>
    public class TeslimEdildiDurum : IGonderiDurum
    {
        public string DurumAdi => "Teslim Edildi";
        public string Aciklama => "Kargonuz başarıyla teslim edildi.";
        public IGonderiDurum? SonrakiDurum() => null;
        public bool IptalEdilabilir() => false;
    }

    /// <summary>
    /// State Pattern — Concrete State: İptal Edildi
    /// Son durum. Geçiş yok. Tekrar iptal edilemez.
    /// </summary>
    public class IptalEdildiDurum : IGonderiDurum
    {
        public string DurumAdi => "İptal Edildi";
        public string Aciklama => "Kargo gönderimi iptal edildi.";
        public IGonderiDurum? SonrakiDurum() => null;
        public bool IptalEdilabilir() => false;
    }

    /// <summary>
    /// Durum adından ilgili State nesnesini oluşturur.
    /// </summary>
    public static class GonderiDurumFactory
    {
        public static IGonderiDurum GetDurum(string durumAdi)
        {
            return durumAdi switch
            {
                "Sipariş Alındı" => new SiparisAlindiDurum(),
                "Hazırlanıyor"   => new HazirlaniyorDurum(),
                "Yolda"          => new YoldaDurum(),
                "Teslim Edildi"  => new TeslimEdildiDurum(),
                "İptal Edildi"   => new IptalEdildiDurum(),
                _                => new SiparisAlindiDurum()
            };
        }
    }
}
