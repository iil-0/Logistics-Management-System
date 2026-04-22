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
