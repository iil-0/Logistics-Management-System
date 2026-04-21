namespace LogiTechAPI.DTOs
{
    public class KargoSiparisRequest
    {
        public string PaketTipi { get; set; } = "Standart";
        public List<string> Ekstralar { get; set; } = new();
        public string TasimaYolu { get; set; } = "Karayolu";
    }

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
}
