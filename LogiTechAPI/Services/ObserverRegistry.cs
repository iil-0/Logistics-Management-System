using System.Collections.Concurrent;
using LogiTechAPI.Observer;

namespace LogiTechAPI.Services
{
    public class ObserverRegistry
    {
        private readonly ConcurrentDictionary<string, List<IShipmentObserver>> _observers = new();

        public void Add(string trackingNo, IShipmentObserver observer)
        {
            var list = _observers.GetOrAdd(trackingNo, _ => new List<IShipmentObserver>());
            lock (list)
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
                observer.Update(message, status);
        }

        public void Remove(string trackingNo)
        {
            _observers.TryRemove(trackingNo, out _);
        }
    }
}
