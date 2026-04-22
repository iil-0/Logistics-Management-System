namespace LogiTechAPI.DTOs.Responses
{
    public class CargoOrderResponse
    {
        public decimal TotalPrice { get; set; }
        public string Description { get; set; } = string.Empty;
        public string PackageType { get; set; } = string.Empty;
        public string TransportMethod { get; set; } = string.Empty;
        public List<string> ExtraServices { get; set; } = new();
        public string ShipmentStatus { get; set; } = string.Empty;
        public List<string> Notifications { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
