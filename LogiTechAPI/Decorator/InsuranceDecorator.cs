// Concrete Decorator — paketin fiyatına 75 TL sigorta ücreti ekler,
// açıklamasına sigorta notu iliştirir. Paketin Name'ini değiştirmez.
using LogiTechAPI.Factory;

namespace LogiTechAPI.Decorator
{
    public class InsuranceDecorator : PackageDecorator
    {
        private const decimal InsuranceFee = 75m;

        public InsuranceDecorator(IPackage package) : base(package) { }

        // Sarılan paketin fiyatına sigorta ücretini ekle (zincir devam eder)
        public override decimal CalculatePrice() => _package.CalculatePrice() + InsuranceFee;
        public override string GetDescription() => _package.GetDescription() + " + Insurance Protection (Full Damage Coverage)";
    }
}
