// Decorator pattern'in SOYUT TABANI. Hem IPackage'i uygular hem de içinde
// IPackage tutar → bir paketi sarıp üzerine yeni davranış ekleme imkânı.
// Recursive composition: dekoratör başka bir dekoratörü de sarabilir.
using LogiTechAPI.Factory;

namespace LogiTechAPI.Decorator
{
    public abstract class PackageDecorator : IPackage
    {
        protected readonly IPackage _package;               // Sarılan paket (saf veya başka dekoratör)

        protected PackageDecorator(IPackage package) => _package = package;

        // Varsayılan delegasyon: çağrıyı içteki nesneye geçir.
        // Concrete dekoratörler gerekirse override eder (fiyata/açıklamaya ek katar).
        public virtual string Name => _package.Name;
        public virtual decimal CalculatePrice() => _package.CalculatePrice();
        public virtual string GetDescription() => _package.GetDescription();
    }
}
