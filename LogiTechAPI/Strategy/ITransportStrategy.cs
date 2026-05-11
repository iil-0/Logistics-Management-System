// Strategy pattern'in SÖZLEŞMESİ. Aynı işi (taşıma ek maliyetini hesaplama)
// farklı algoritmalarla yapan sınıfların ortak arabirimi.
// State'ten farkı: durum kendi değişmez, istemci stratejisini DIŞARIDAN seçer.
namespace LogiTechAPI.Strategy
{
    public interface ITransportStrategy
    {
        string Name { get; }
        decimal CalculateExtraCost(decimal basePrice);    // Baz fiyat üzerinden ek ücret
        string GetDescription();
    }
}
