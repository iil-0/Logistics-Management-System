namespace LogiTechAPI.Factory
{
    public class FragilePackage : IPaket
    {
        public string Name => "Fragile";
        public decimal CalculatePrice() => 120m;
        public string GetDescription() => "Fragile Package - Special packaging for breakable items";
    }
}
