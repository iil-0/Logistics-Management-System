// Concrete Observer — bellek-içi log gözlemcisi.
// Gerçek bir kanal kullanmaz; gelen bildirimleri zaman damgalı listede tutar.
// Debug/test için faydalı, üretimde DB log'u veya gerçek bir kanal tercih edilir.
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
