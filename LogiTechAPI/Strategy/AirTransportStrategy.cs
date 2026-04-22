namespace LogiTechAPI.Strategy
{
    public class AirTransportStrategy : ITransportStrategy
    {
        public string Name => "Air";
        public decimal CalculateExtraCost(decimal basePrice) => basePrice * 0.80m;
        public string GetDescription() => "Air Transport - Express Delivery (1-2 Business Days)";
    }
}
