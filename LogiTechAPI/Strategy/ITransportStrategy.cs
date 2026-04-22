namespace LogiTechAPI.Strategy
{
    public interface ITransportStrategy
    {
        string Name { get; }
        decimal CalculateExtraCost(decimal basePrice);
        string GetDescription();
    }
}
