// Concrete Strategy — Kara yolu. Hız/fiyat dengesi orta, %20 ek ücret.
// Factory'de varsayılan seçenek.
namespace LogiTechAPI.Strategy
{
    public class LandTransportStrategy : ITransportStrategy
    {
        public string Name => "Land";
        public decimal CalculateExtraCost(decimal basePrice) => basePrice * 0.20m;
        public string GetDescription() => "Land Transport - Economic Delivery (3-5 Business Days)";
    }
}
