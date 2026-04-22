namespace LogiTechAPI.DTOs.Responses
{
    public class StatusHistoryResponse
    {
        public string Status { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
