namespace LogiTechAPI.Factory
{
    public interface IPaket
    {
        string Name { get; }
        decimal CalculatePrice();
        string GetDescription();
    }
}
