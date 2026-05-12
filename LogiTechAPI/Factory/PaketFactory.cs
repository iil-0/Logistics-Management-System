// Factory Method pattern'de CLIENT yardımcısı: string anahtara göre uygun
// Concrete Creator'ı seçer ve ona CreatePackage() çağrısını DELEGE eder.
// Önemli fark (Simple Factory'den): "new XPackage()" satırı burada YOKTUR —
// her ürünün yaratımı kendi Concrete Creator'ında yapılır. Bu sınıf yalnız
// "hangi creator?" sorusunun cevabını verir, "ürünü nasıl yaratırım"ı değil.
namespace LogiTechAPI.Factory
{
    public class PaketFactory
    {
        // String → uygun Creator eşlemesi. Yeni paket türü eklemek için:
        // 1. Concrete Package yaz, 2. Concrete Creator yaz, 3. buraya bir satır ekle.
        private readonly Dictionary<string, IPaketCreator> _creators = new()
        {
            ["standard"]  = new StandardPackageCreator(),
            ["fragile"]   = new FragilePackageCreator(),
            ["heavyload"] = new HeavyLoadPackageCreator(),
            // Backward compatibility: eski Türkçe değerler
            ["standart"]  = new StandardPackageCreator(),
            ["hassas"]    = new FragilePackageCreator(),
            ["agiryuk"]   = new HeavyLoadPackageCreator(),
        };

        private readonly IPaketCreator _defaultCreator = new StandardPackageCreator();

        public IPaket CreatePackage(string packageType)
        {
            var key = packageType?.ToLower() ?? string.Empty;
            // Uygun creator bulunursa onun Factory Method'unu çağır;
            // bulunamazsa güvenli varsayılan creator (Standard) kullanılır.
            var creator = _creators.TryGetValue(key, out var found) ? found : _defaultCreator;
            return creator.CreatePackage();           // Factory Method çağrısı — ürün burada doğar
        }
    }
}
