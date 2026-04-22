namespace LogiTechAPI.Strategy
{
    public class SeaTransportStrategy : ITransportStrategy
    {
        public string Name => "Sea";
        public decimal CalculateExtraCost(decimal basePrice) => basePrice * 0.10m;
        public string GetDescription() => "Sea Transport - Most Economic (7-14 Business Days)";
    }
}
