// Concrete Component — Factory'nin ürettiği temel paket türlerinden biri.
// Hiçbir ek hizmet içermez, en ucuz baz fiyat (50 TL).
namespace LogiTechAPI.Factory
{
    public class StandardPackage : IPackage
    {
        public string Name => "Standard";
        public decimal CalculatePrice() => 50m;
        public string GetDescription() => "Standard Package - Normal cargo services";
    }
}
