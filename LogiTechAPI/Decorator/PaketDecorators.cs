using LogiTechAPI.Factory;

namespace LogiTechAPI.Decorator
{
    // Base Decorator
    public abstract class PaketDecorator : IPaket
    {
        protected readonly IPaket _paket;

        protected PaketDecorator(IPaket paket)
        {
            _paket = paket;
        }

        public virtual string GetAciklama() => _paket.GetAciklama();
        public virtual decimal GetFiyat() => _paket.GetFiyat();
        public virtual string GetTip() => _paket.GetTip();
    }

    // Sigorta Decorator
    public class SigortaDecorator : PaketDecorator
    {
        private const decimal SigortaUcreti = 75m;

        public SigortaDecorator(IPaket paket) : base(paket) { }

        public override string GetAciklama() =>
            _paket.GetAciklama() + " + Sigorta Güvencesi (Tam Hasar Koruma)";

        public override decimal GetFiyat() => _paket.GetFiyat() + SigortaUcreti;
    }

    // Hızlı Teslimat Decorator
    public class HizliTeslimatDecorator : PaketDecorator
    {
        private const decimal HizliTeslimatUcreti = 100m;

        public HizliTeslimatDecorator(IPaket paket) : base(paket) { }

        public override string GetAciklama() =>
            _paket.GetAciklama() + " + Hızlı Teslimat (Aynı Gün / 24 Saat)";

        public override decimal GetFiyat() => _paket.GetFiyat() + HizliTeslimatUcreti;
    }
}
