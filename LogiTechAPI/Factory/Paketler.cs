namespace LogiTechAPI.Factory
{
    // Standart Paket
    public class StandartPaket : IPaket
    {
        public string GetAciklama() => "Standart Paket - Normal kargo hizmetleri";
        public decimal GetFiyat() => 50m;
        public string GetTip() => "Standart";
    }

    // Hassas Paket
    public class HassasPaket : IPaket
    {
        public string GetAciklama() => "Hassas Paket - Kırılabilir ürünler için özel ambalaj";
        public decimal GetFiyat() => 120m;
        public string GetTip() => "Hassas";
    }

    // Ağır Yük Paketi
    public class AgirYukPaket : IPaket
    {
        public string GetAciklama() => "Ağır Yük Paketi - 50kg üzeri endüstriyel kargo";
        public decimal GetFiyat() => 250m;
        public string GetTip() => "AgirYuk";
    }
}
