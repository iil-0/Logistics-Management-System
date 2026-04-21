using System.Collections.Concurrent;
using LogiTechAPI.Models;

namespace LogiTechAPI.Services
{
    public class GonderiService
    {
        private readonly ConcurrentDictionary<string, Gonderi> _gonderiler = new();
        private int _nextId = 1;
        private int _takipSayac = 1000;

        public string TakipNoUret()
        {
            var tarih = DateTime.Now.ToString("yyyyMMdd");
            var sayac = Interlocked.Increment(ref _takipSayac);
            return $"LT-{tarih}-{sayac}";
        }

        public void Ekle(Gonderi gonderi)
        {
            gonderi.Id = _nextId++;
            _gonderiler.TryAdd(gonderi.TakipNo, gonderi);
        }

        public Gonderi? TakipNoIleBul(string takipNo)
        {
            _gonderiler.TryGetValue(takipNo, out var gonderi);
            return gonderi;
        }

        public List<Gonderi> KullaniciGonderileri(int userId)
        {
            return _gonderiler.Values
                .Where(g => g.UserId == userId)
                .OrderByDescending(g => g.OlusturulmaTarihi)
                .ToList();
        }
    }
}
