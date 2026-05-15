// Concrete Component — Factory'nin ürettiği kırılgan eşya paketi (120 TL).
// Ekstra paketleme maliyeti yansıtılır.
namespace LogiTechAPI.Factory
{
    public class FragilePackage : IPackage
    {
        public string Name => "Fragile";
        public decimal CalculatePrice() => 120m;
        public string GetDescription() => "Fragile Package - Special packaging for breakable items";
    }
}
