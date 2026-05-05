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
