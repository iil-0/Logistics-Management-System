namespace LogiTechAPI.DTOs.Responses
{
    public class ShipmentResponse
    {
        public int Id { get; set; }
        public string TrackingNo { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverAddress { get; set; } = string.Empty;
        public string ReceiverCity { get; set; } = string.Empty;
        public string PackageType { get; set; } = string.Empty;
        public List<string> Extras { get; set; } = new();
        public string TransportMethod { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsCancellable { get; set; }
        public List<StatusHistoryResponse> StatusHistory { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
