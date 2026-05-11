// Factory + Decorator pattern'in ortak SÖZLEŞMESİ.
// Tüm paket türleri ve dekoratörler bunu uygular → polimorfizm sayesinde
// fabrika string'den IPaket üretir, dekoratör IPaket'i sarıp yine IPaket döner.
namespace LogiTechAPI.Factory
{
    public interface IPaket
    {
        string Name { get; }                  // Paket tipi adı (Standard / Fragile / HeavyLoad)
        decimal CalculatePrice();             // Decorator zincirinde her halka kendi ücretini ekler
        string GetDescription();              // Açıklama metni (dekoratörler uzatır)
    }
}
