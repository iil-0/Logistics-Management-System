using LogiTechAPI.Data;
using LogiTechAPI.Models;
using Microsoft.EntityFrameworkCore;
using LogiTechAPI.Observer;
using System.Collections.Concurrent;

namespace LogiTechAPI.Services
{
    public class GonderiService
    {
        private readonly AppDbContext _context;
        private readonly ConcurrentDictionary<string, List<IShipmentObserver>> _observers = new();

        public GonderiService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddShipment(Gonderi shipment)
        {
            _context.Shipments.Add(shipment);
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

        // Observer Pattern methods
        public void AddObserver(string trackingNo, IShipmentObserver observer)
        {
            if (!_observers.ContainsKey(trackingNo))
                _observers[trackingNo] = new List<IShipmentObserver>();
            
            _observers[trackingNo].Add(observer);
        }

        public void NotifyObservers(string trackingNo, string message, string status)
        {
            if (_observers.TryGetValue(trackingNo, out var observers))
            {
                foreach (var observer in observers)
                    observer.Update(message, status);
            }
        }
    }
}
