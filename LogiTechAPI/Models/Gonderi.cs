using LogiTechAPI.State;

namespace LogiTechAPI.Models
{
    public class Gonderi
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string TakipNo { get; set; } = string.Empty;

        // Gönderici bilgileri (User'dan otomatik dolar)
        public string GondericiAd { get; set; } = string.Empty;
        public string GondericiEmail { get; set; } = string.Empty;
        public string GondericiTelefon { get; set; } = string.Empty;

        // Alıcı bilgileri
        public string AliciAd { get; set; } = string.Empty;
        public string AliciAdres { get; set; } = string.Empty;
        public string AliciTelefon { get; set; } = string.Empty;
        public string AliciSehir { get; set; } = string.Empty;

        // Kargo detayları
        public string PaketTipi { get; set; } = string.Empty;
        public List<string> Ekstralar { get; set; } = new();
        public string TasimaYolu { get; set; } = string.Empty;
        public decimal ToplamFiyat { get; set; }
        public string Aciklama { get; set; } = string.Empty;

        // State Pattern — Durum yönetimi
        public string Durum { get; set; } = "Sipariş Alındı";
        public List<DurumGecmisi> DurumGecmisi { get; set; } = new();

        public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;
    }

    public class DurumGecmisi
    {
        public string Durum { get; set; } = string.Empty;
        public DateTime Tarih { get; set; } = DateTime.Now;
        public string Aciklama { get; set; } = string.Empty;
    }
}
