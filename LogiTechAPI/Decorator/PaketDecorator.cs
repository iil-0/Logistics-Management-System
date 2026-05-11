// Decorator pattern'in SOYUT TABANI. Hem IPaket'i uygular hem de içinde
// IPaket tutar → bir paketi sarıp üzerine yeni davranış ekleme imkânı.
// Recursive composition: dekoratör başka bir dekoratörü de sarabilir.
using LogiTechAPI.Factory;

namespace LogiTechAPI.Decorator
{
    public abstract class PaketDecorator : IPaket
    {
        protected readonly IPaket _paket;                   // Sarılan paket (saf veya başka dekoratör)

        protected PaketDecorator(IPaket paket) => _paket = paket;

        // Varsayılan delegasyon: çağrıyı içteki nesneye geçir.
        // Concrete dekoratörler gerekirse override eder (fiyata/açıklamaya ek katar).
        public virtual string Name => _paket.Name;
        public virtual decimal CalculatePrice() => _paket.CalculatePrice();
        public virtual string GetDescription() => _paket.GetDescription();
    }
}
