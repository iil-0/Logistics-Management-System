// Concrete Component — Factory'nin ürettiği 50kg+ endüstriyel paket (250 TL).
// En pahalı baz fiyat: forklift, palet, özel araç gerektirir.
namespace LogiTechAPI.Factory
{
    public class HeavyLoadPackage : IPackage
    {
        public string Name => "HeavyLoad";
        public decimal CalculatePrice() => 250m;
        public string GetDescription() => "Heavy Load Package - Industrial cargo over 50kg";
    }
}
