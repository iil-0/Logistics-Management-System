namespace LogiTechAPI.DTOs.Requests
{
    public class CargoOrderRequest
    {
        public string PackageType { get; set; } = "Standard";
        public List<string> Extras { get; set; } = new();
        public string TransportMethod { get; set; } = "Land";
    }
}
