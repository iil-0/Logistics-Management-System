namespace LogiTechAPI.Factory
{
    public class HeavyLoadPackage : IPaket
    {
        public string Name => "HeavyLoad";
        public decimal CalculatePrice() => 250m;
        public string GetDescription() => "Heavy Load Package - Industrial cargo over 50kg";
    }
}
