// Kargo iş mantığı servisi. Command pattern'de RECEIVER rolü — komutlar
// (CreateShipment/UpdateStatus/CancelShipment) işi buraya delege eder. Ayrıca Observer
// pattern'in Subject'ine (ObserverRegistry) ince bir Facade cephesi sunar.
using LogiTechAPI.Data;
using LogiTechAPI.Models;
using Microsoft.EntityFrameworkCore;
using LogiTechAPI.Observer;

namespace LogiTechAPI.Services
{
    public class ShipmentService
    {
        private readonly AppDbContext _context;          // EF Core DbContext (Scoped)
        private readonly ObserverRegistry _registry;     // Observer Subject (Singleton)

        public ShipmentService(AppDbContext context, ObserverRegistry registry)
        {
            _context = context;
            _registry = registry;
        }

        // INSERT — yeni gönderi
        public async Task AddShipment(Shipment shipment)
        {
            _context.Shipments.Add(shipment);
            await _context.SaveChangesAsync();
        }

        // UPDATE — değişiklikleri commit et
        public async Task UpdateShipment(Shipment shipment)
        {
            _context.Shipments.Update(shipment);
            await _context.SaveChangesAsync();
        }

        // UNDO altyapısı: tek transaction'da status revert + history satırı sil.
        // EF'in change tracker'ı koleksiyon silmeyi yakalamadığı için DbSet üzerinden silinir.
        public async Task<bool> RevertStatus(string trackingNo, string oldStatus, int? historyIdToDelete)
        {
            var shipment = await _context.Shipments
                .FirstOrDefaultAsync(g => g.TrackingNo == trackingNo);
            if (shipment == null) return false;

            shipment.Status = oldStatus;                // Tracked entity → otomatik Modified

            if (historyIdToDelete.HasValue)
            {
                var history = await _context.StatusHistories.FindAsync(historyIdToDelete.Value);
                if (history != null) _context.StatusHistories.Remove(history);   // DbSet üzerinden sil
            }

            await _context.SaveChangesAsync();          // Tek atomik commit
            return true;
        }

        // SELECT TOP 1 ... WHERE TrackingNo = ?  (StatusHistory eager-loaded)
        public async Task<Shipment?> GetByTrackingNo(string trackingNo)
        {
            return await _context.Shipments
                .Include(g => g.StatusHistory)          // JOIN ile alt tablo da gelir
                .FirstOrDefaultAsync(g => g.TrackingNo == trackingNo);
        }

        // DELETE — cascade ile StatusHistory satırları da silinir (AppDbContext yapılandırması)
        public async Task DeleteShipment(string trackingNo)
        {
            var shipment = await GetByTrackingNo(trackingNo);
            if (shipment != null)
            {
                _context.Shipments.Remove(shipment);
                await _context.SaveChangesAsync();
            }
        }

        // Kullanıcının kendi kargoları (en yeniden eskiye)
        public async Task<List<Shipment>> GetUserShipments(int userId)
        {
            return await _context.Shipments
                .Where(g => g.UserId == userId)
                .Include(g => g.StatusHistory)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        // Admin için tüm kargolar
        public async Task<List<Shipment>> GetAllShipments()
        {
            return await _context.Shipments
                .Include(g => g.StatusHistory)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        // ─── Observer Pattern Facade ─────────────────────────────────────────
        // Gerçek iş ObserverRegistry'de yapılır; bu metotlar sadece çağrıyı yönlendirir.

        public void AddObserver(string trackingNo, IShipmentObserver observer)
            => _registry.Add(trackingNo, observer);

        public void NotifyObservers(string trackingNo, string message, string status)
            => _registry.Notify(trackingNo, message, status);

        public void RemoveObservers(string trackingNo)
            => _registry.Remove(trackingNo);
    }
}
