// Concrete Decorator — paketin fiyatına 100 TL hızlı teslimat ücreti ekler.
// Sigorta ile birlikte zincirlenebilir: HizliTeslimat(Sigorta(Fragile)) → 120+75+100 = 295 TL.
using LogiTechAPI.Factory;

namespace LogiTechAPI.Decorator
{
    public class HizliTeslimatDecorator : PaketDecorator
    {
        private const decimal FastDeliveryFee = 100m;

        public HizliTeslimatDecorator(IPaket paket) : base(paket) { }

        public override decimal CalculatePrice() => _paket.CalculatePrice() + FastDeliveryFee;
        public override string GetDescription() => _paket.GetDescription() + " + Fast Delivery (Same Day / 24 Hours)";
    }
}
