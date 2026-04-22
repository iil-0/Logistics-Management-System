using LogiTechAPI.Factory;

namespace LogiTechAPI.Decorator
{
    public class SigortaDecorator : PaketDecorator
    {
        private const decimal InsuranceFee = 75m;

        public SigortaDecorator(IPaket paket) : base(paket) { }

        public override decimal CalculatePrice() => _paket.CalculatePrice() + InsuranceFee;
        public override string GetDescription() => _paket.GetDescription() + " + Insurance Protection (Full Damage Coverage)";
    }
}
