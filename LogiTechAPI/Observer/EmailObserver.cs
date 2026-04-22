namespace LogiTechAPI.Observer
{
    public class EmailObserver : IShipmentObserver
    {
        public List<string> Messages { get; } = new();

        public void Update(string message, string status)
        {
            var log = $"📧 Email sent: '{message}' - Status: {status}";
            Messages.Add(log);
        }
    }
}
