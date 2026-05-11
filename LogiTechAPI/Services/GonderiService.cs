using LogiTechAPI.Data;
using LogiTechAPI.Models;
using Microsoft.EntityFrameworkCore;
using LogiTechAPI.Observer;

namespace LogiTechAPI.Services
{
    public class GonderiService
    {
        private readonly AppDbContext _context;
        private readonly ObserverRegistry _registry;

        public GonderiService(AppDbContext context, ObserverRegistry registry)
        {
            _context = context;
            _registry = registry;
        }

        public async Task AddShipment(Gonderi shipment)
        {
            _context.Shipments.Add(shipment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateShipment(Gonderi shipment)
        {
            _context.Shipments.Update(shipment);
            await _context.SaveChangesAsync();
        }

        // Undo akışı için: shipment.Status'u eski değere döndürür ve istenirse
        // bir StatusHistory satırını da DB'den TAMAMEN siler. Tek transaction
        // içinde yapılır (tek SaveChanges) — atomik.
        public async Task<bool> RevertStatus(string trackingNo, string oldStatus, int? historyIdToDelete)
        {
            var shipment = await _context.Shipments
                .FirstOrDefaultAsync(g => g.TrackingNo == trackingNo);
            if (shipment == null) return false;

            shipment.Status = oldStatus;

            if (historyIdToDelete.HasValue)
            {
                var history = await _context.StatusHistories.FindAsync(historyIdToDelete.Value);
                if (history != null) _context.StatusHistories.Remove(history);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Gonderi?> GetByTrackingNo(string trackingNo)
        {
            return await _context.Shipments
                .Include(g => g.StatusHistory)
                .FirstOrDefaultAsync(g => g.TrackingNo == trackingNo);
        }

        public async Task DeleteShipment(string trackingNo)
        {
            var shipment = await GetByTrackingNo(trackingNo);
            if (shipment != null)
            {
                _context.Shipments.Remove(shipment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Gonderi>> GetUserShipments(int userId)
        {
            return await _context.Shipments
                .Where(g => g.UserId == userId)
                .Include(g => g.StatusHistory)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Gonderi>> GetAllShipments()
        {
            return await _context.Shipments
                .Include(g => g.StatusHistory)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        // Observer Pattern — singleton kayit defterine devrediyor
        public void AddObserver(string trackingNo, IShipmentObserver observer)
            => _registry.Add(trackingNo, observer);

        public void NotifyObservers(string trackingNo, string message, string status)
            => _registry.Notify(trackingNo, message, status);

        public void RemoveObservers(string trackingNo)
            => _registry.Remove(trackingNo);
    }
}
