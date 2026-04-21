namespace LogiTechAPI.DTOs
{
    // ─── İstek Modelleri ──────────────────────────────────────────────────────

    public class KargoSiparisRequest
    {
        public string PaketTipi { get; set; } = "Standart";
        public List<string> Ekstralar { get; set; } = new();
        public string TasimaYolu { get; set; } = "Karayolu";
    }

    public class GonderiRequest
    {
        // Alıcı bilgileri
        public string AliciAd { get; set; } = string.Empty;
        public string AliciAdres { get; set; } = string.Empty;
        public string AliciTelefon { get; set; } = string.Empty;
        public string AliciSehir { get; set; } = string.Empty;

        // Kargo detayları
        public string PaketTipi { get; set; } = "Standart";
        public List<string> Ekstralar { get; set; } = new();
        public string TasimaYolu { get; set; } = "Karayolu";
    }

    public class KayitRequest
    {
        public string Ad { get; set; } = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        public string Sifre { get; set; } = string.Empty;
    }

    public class GirisRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Sifre { get; set; } = string.Empty;
    }

    // ─── Yanıt Modelleri ──────────────────────────────────────────────────────

    public class KargoSiparisResponse
    {
        public decimal ToplamFiyat { get; set; }
        public string Aciklama { get; set; } = string.Empty;
        public string PaketTipi { get; set; } = string.Empty;
        public string TasimaYolu { get; set; } = string.Empty;
        public List<string> EkstraHizmetler { get; set; } = new();
        public string KargoDurumu { get; set; } = string.Empty;
        public List<string> Bildirimler { get; set; } = new();
        public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;
    }

    public class GonderiResponse
    {
        public int Id { get; set; }
        public string TakipNo { get; set; } = string.Empty;
        public string GondericiAd { get; set; } = string.Empty;
        public string AliciAd { get; set; } = string.Empty;
        public string AliciAdres { get; set; } = string.Empty;
        public string AliciSehir { get; set; } = string.Empty;
        public string PaketTipi { get; set; } = string.Empty;
        public List<string> Ekstralar { get; set; } = new();
        public string TasimaYolu { get; set; } = string.Empty;
        public decimal ToplamFiyat { get; set; }
        public string Durum { get; set; } = string.Empty;
        public bool IptalEdilabilir { get; set; }
        public List<DurumGecmisiResponse> DurumGecmisi { get; set; } = new();
        public DateTime OlusturulmaTarihi { get; set; }
    }

    public class DurumGecmisiResponse
    {
        public string Durum { get; set; } = string.Empty;
        public DateTime Tarih { get; set; }
        public string Aciklama { get; set; } = string.Empty;
    }

    public class UserResponse
    {
        public int Id { get; set; }
        public string Ad { get; set; } = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
    }
}
