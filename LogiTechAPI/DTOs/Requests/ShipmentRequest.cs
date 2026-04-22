namespace LogiTechAPI.DTOs.Requests
{
    public class ShipmentRequest
    {
        // Receiver information
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverAddress { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;
        public string ReceiverCity { get; set; } = string.Empty;

        // Shipment details
        public string PackageType { get; set; } = "Standard";
        public List<string> Extras { get; set; } = new();
        public string TransportMethod { get; set; } = "Land";
        public string? Notes { get; set; }
    }
}
