namespace LogiTechAPI.Observer
{
    public class NotificationObserver : IShipmentObserver
    {
        public List<string> Messages { get; } = new();

        public void Update(string message, string status)
        {
            var log = $"[{DateTime.Now:HH:mm:ss}] 💫 {status}: {message}";
            Messages.Add(log);
        }
    }
}
