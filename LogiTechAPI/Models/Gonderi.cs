using LogiTechAPI.State;

namespace LogiTechAPI.Models
{
    public class Gonderi
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string TrackingNo { get; set; } = string.Empty;

        // Sender information
        public string SenderName { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderPhone { get; set; } = string.Empty;

        // Receiver information
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverAddress { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;
        public string ReceiverCity { get; set; } = string.Empty;

        // Shipment details
        public string PackageType { get; set; } = string.Empty;
        public List<string> Extras { get; set; } = new();
        public string TransportMethod { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public string Notes { get; set; } = string.Empty;

        // Status management
        public string Status { get; set; } = "Order Received";
        public List<StatusHistory> StatusHistory { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class StatusHistory
    {
        public int Id { get; set; } // Added Primary Key
        public string Status { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;
        public string Message { get; set; } = string.Empty;
    }
}
