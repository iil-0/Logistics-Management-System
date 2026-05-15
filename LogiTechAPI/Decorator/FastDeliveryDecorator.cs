// Concrete Decorator — paketin fiyatına 100 TL hızlı teslimat ücreti ekler.
// Sigorta ile birlikte zincirlenebilir: FastDelivery(Insurance(Fragile))
using LogiTechAPI.Factory;

namespace LogiTechAPI.Decorator
{
    public class FastDeliveryDecorator : PackageDecorator
    {
        private const decimal FastDeliveryFee = 100m;

        public FastDeliveryDecorator(IPackage package) : base(package) { }

        public override decimal CalculatePrice() => _package.CalculatePrice() + FastDeliveryFee;
        public override string GetDescription() => _package.GetDescription() + " + Fast Delivery (Same Day / 24 Hours)";
    }
}
