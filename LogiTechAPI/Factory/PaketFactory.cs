// Simple Factory — string parametresine göre uygun IPaket somut nesnesini üretir.
// Üst katman (Command) "new StandardPackage()" yazmaz, fabrikadan ister; yeni paket
// türü eklendiğinde sadece bu switch güncellenir (Open/Closed Principle).
namespace LogiTechAPI.Factory
{
    public class PaketFactory
    {
        public IPaket CreatePackage(string packageType)
        {
            return packageType?.ToLower() switch
            {
                "standard"  => new StandardPackage(),
                "fragile"   => new FragilePackage(),
                "heavyload" => new HeavyLoadPackage(),
                // Backward compatibility: eski TR değerler
                "standart"  => new StandardPackage(),
                "hassas"    => new FragilePackage(),
                "agiryuk"   => new HeavyLoadPackage(),
                _           => new StandardPackage()        // Bilinmeyen → güvenli varsayılan
            };
        }
    }
}
