// Concrete Decorator — paketin fiyatına 75 TL sigorta ücreti ekler,
// açıklamasına sigorta notu iliştirir. Paketin Name'ini değiştirmez.
using LogiTechAPI.Factory;

namespace LogiTechAPI.Decorator
{
    public class SigortaDecorator : PaketDecorator
    {
        private const decimal InsuranceFee = 75m;

        public SigortaDecorator(IPaket paket) : base(paket) { }

        // Sarılan paketin fiyatına sigorta ücretini ekle (zincir devam eder)
        public override decimal CalculatePrice() => _paket.CalculatePrice() + InsuranceFee;
        public override string GetDescription() => _paket.GetDescription() + " + Insurance Protection (Full Damage Coverage)";
    }
}
