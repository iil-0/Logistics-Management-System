// Factory + Decorator pattern'in ortak SÖZLEŞMESİ.
// Tüm paket türleri ve dekoratörler bunu uygular → polimorfizm sayesinde
// fabrika string'den IPackage üretir, dekoratör IPackage'i sarıp yine IPackage döner.
namespace LogiTechAPI.Factory
{
    public interface IPackage
    {
        string Name { get; }                  // Paket tipi adı (Standard / Fragile / HeavyLoad)
        decimal CalculatePrice();             // Decorator zincirinde her halka kendi ücretini ekler
        string GetDescription();              // Açıklama metni (dekoratörler uzatır)
    }
}
