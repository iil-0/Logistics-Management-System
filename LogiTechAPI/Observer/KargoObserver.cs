namespace LogiTechAPI.Observer
{
    // Observer Interface
    public interface IKargoObserver
    {
        void Update(string mesaj, string durum);
    }

    // Subject Interface
    public interface IKargoTakip
    {
        void Subscribe(IKargoObserver observer);
        void Unsubscribe(IKargoObserver observer);
        void Notify(string mesaj, string durum);
    }

    // Concrete Subject - Kargo Takip Sistemi
    public class KargoTakipServisi : IKargoTakip
    {
        private readonly List<IKargoObserver> _observers = new();
        private string _mevcutDurum = "Beklemede";

        public void Subscribe(IKargoObserver observer) => _observers.Add(observer);
        public void Unsubscribe(IKargoObserver observer) => _observers.Remove(observer);

        public void Notify(string mesaj, string durum)
        {
            _mevcutDurum = durum;
            foreach (var observer in _observers)
            {
                observer.Update(mesaj, durum);
            }
        }

        public string GetDurum() => _mevcutDurum;
    }

    // Concrete Observer - Bildirim Logger'ı
    public class BildirimObserver : IKargoObserver
    {
        public List<string> Mesajlar { get; } = new();

        public void Update(string mesaj, string durum)
        {
            var log = $"[{DateTime.Now:HH:mm:ss}] 📦 {durum}: {mesaj}";
            Mesajlar.Add(log);
        }
    }

    // Concrete Observer - Email Simülasyonu
    public class EmailObserver : IKargoObserver
    {
        public List<string> Mesajlar { get; } = new();

        public void Update(string mesaj, string durum)
        {
            var log = $"📧 E-posta gönderildi: '{mesaj}' - Durum: {durum}";
            Mesajlar.Add(log);
        }
    }
}
