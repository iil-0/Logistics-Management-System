// Observer pattern'in SUBJECT'i (yayın yapan). Her trackingNo için kayıtlı
// gözlemcileri tutar ve durum değiştiğinde hepsini sırayla Update eder.
// Singleton — DI'da AddSingleton ile kayıtlı; tüm istekler aynı listeyi paylaşır.
using System.Collections.Concurrent;

namespace LogiTechAPI.Observer
{
    public class ObserverRegistry
    {
        // TrackingNo → o gönderiye abone observer'lar.
        // ConcurrentDictionary: eşzamanlı Add/Remove güvenliği.
        private readonly ConcurrentDictionary<string, List<IShipmentObserver>> _observers = new();

        public void Add(string trackingNo, IShipmentObserver observer)
        {
            // GetOrAdd atomik: anahtar yoksa yeni liste yarat, varsa mevcutu dön
            var list = _observers.GetOrAdd(trackingNo, _ => new List<IShipmentObserver>());
            lock (list)                                  // List<T> thread-safe değil
            {
                list.Add(observer);
            }
        }

        public void Notify(string trackingNo, string message, string status)
        {
            if (!_observers.TryGetValue(trackingNo, out var list)) return;

            IShipmentObserver[] snapshot;
            lock (list)
            {
                snapshot = list.ToArray();
            }

            foreach (var observer in snapshot)
                observer.Update(message, status);        // Concrete observer (Email/Notification) çağrılır
        }

        public void Remove(string trackingNo)
        {
            _observers.TryRemove(trackingNo, out _);     // Anahtar yoksa hata vermeden geç
        }
    }
}
