namespace LogiTechAPI.Factory
{
    public class StandardPackage : IPaket
    {
        public string Name => "Standard";
        public decimal CalculatePrice() => 50m;
        public string GetDescription() => "Standard Package - Normal cargo services";
    }
}
