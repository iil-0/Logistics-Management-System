using LogiTechAPI.Models;
using LogiTechAPI.Factory;
using LogiTechAPI.Decorator;
using LogiTechAPI.Strategy;
using LogiTechAPI.Observer;
using LogiTechAPI.Services;
using LogiTechAPI.Settings;
using LogiTechAPI.DTOs.Requests;
using Microsoft.Extensions.DependencyInjection;

namespace LogiTechAPI.Command
{
    public class GonderiOlusturCommand : IKargoCommand
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly PaketFactory _factory;             // Singleton — direkt tutmak güvenli
        private readonly ShipmentRequest _request;
        private readonly int _userId;
        private readonly User _user;
        private readonly EmailSettings _emailSettings;
        private Gonderi? _createdShipment;

        public string KomutAdi => "Create Shipment";

        public GonderiOlusturCommand(
            IServiceScopeFactory scopeFactory,
            PaketFactory factory,
            ShipmentRequest request,
            int userId,
            User user,
            EmailSettings emailSettings)
        {
            _scopeFactory = scopeFactory;
            _factory = factory;
            _request = request;
            _userId = userId;
            _user = user;
            _emailSettings = emailSettings;
        }

        public async Task<KomutSonuc> Execute()
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var gonderiService = scope.ServiceProvider.GetRequiredService<GonderiService>();

                // REDO durumu: Undo'da silinen gönderiyi aynı TrackingNo ile geri yükle.
                if (_createdShipment != null)
                {
                    _createdShipment.Id = 0;
                    foreach (var h in _createdShipment.StatusHistory) h.Id = 0;

                    gonderiService.AddObserver(_createdShipment.TrackingNo, new NotificationObserver());
                    if (!string.IsNullOrWhiteSpace(_user.Email))
                    {
                        gonderiService.AddObserver(
                            _createdShipment.TrackingNo,
                            new EmailObserver(_user.Email, _createdShipment.TrackingNo, _emailSettings));
                    }

                    await gonderiService.AddShipment(_createdShipment);
                    return new KomutSonuc { Basarili = true, Mesaj = "Shipment re-created!", Gonderi = _createdShipment };
                }

                // 1. Factory
                IPaket paket = _factory.CreatePackage(_request.PackageType);

                // 2. Decorator
                if (_request.Extras != null)
                {
                    foreach (var extra in _request.Extras)
                    {
                        if (extra == "Insurance") paket = new SigortaDecorator(paket);
                        if (extra == "FastDelivery") paket = new HizliTeslimatDecorator(paket);
                    }
                }

                // 3. Strategy
                ITransportStrategy strategy = TransportStrategyFactory.GetStrategy(_request.TransportMethod);
                decimal basePrice = paket.CalculatePrice();
                decimal extraCost = strategy.CalculateExtraCost(basePrice);
                decimal finalPrice = basePrice + extraCost;

                // 4. Model creation
                var shipment = new Gonderi
                {
                    UserId = _userId,
                    TrackingNo = "LT-" + DateTime.UtcNow.ToString("yyyyMMdd") + "-" + new Random().Next(1000, 9999),
                    SenderName = $"{_user.FirstName ?? string.Empty} {_user.LastName ?? string.Empty}".Trim(),
                    SenderEmail = _user.Email ?? string.Empty,
                    SenderPhone = _user.Phone ?? string.Empty,
                    ReceiverName = _request.ReceiverName ?? string.Empty,
                    ReceiverAddress = _request.ReceiverAddress ?? string.Empty,
                    ReceiverPhone = _request.ReceiverPhone ?? string.Empty,
                    ReceiverCity = _request.ReceiverCity ?? string.Empty,
                    PackageType = paket.Name,
                    Extras = _request.Extras ?? new List<string>(),
                    TransportMethod = strategy.Name ?? string.Empty,
                    TotalPrice = finalPrice,
                    Notes = _request.Notes ?? string.Empty
                };

                // 5. State & Observer
                gonderiService.AddObserver(shipment.TrackingNo, new NotificationObserver());

                if (!string.IsNullOrWhiteSpace(_user.Email))
                {
                    gonderiService.AddObserver(
                        shipment.TrackingNo,
                        new EmailObserver(_user.Email, shipment.TrackingNo, _emailSettings));
                }

                shipment.Status = "Order Received";
                shipment.StatusHistory.Add(new StatusHistory
                {
                    Status = shipment.Status,
                    Message = "Shipment created successfully.",
                    Date = DateTime.UtcNow
                });

                // 6. Save
                await gonderiService.AddShipment(shipment);
                _createdShipment = shipment;

                return new KomutSonuc { Basarili = true, Mesaj = "Shipment created successfully!", Gonderi = shipment };
            }
            catch (Exception ex)
            {
                var innerMsg = ex.InnerException != null ? " | Inner: " + ex.InnerException.Message : "";
                return new KomutSonuc { Basarili = false, Mesaj = "Error: " + ex.Message + innerMsg };
            }
        }

        public async Task<KomutSonuc> Undo()
        {
            if (_createdShipment == null)
                return new KomutSonuc { Basarili = false, Mesaj = "Nothing to undo!" };

            using var scope = _scopeFactory.CreateScope();
            var gonderiService = scope.ServiceProvider.GetRequiredService<GonderiService>();

            gonderiService.RemoveObservers(_createdShipment.TrackingNo);
            await gonderiService.DeleteShipment(_createdShipment.TrackingNo);
            return new KomutSonuc { Basarili = true, Mesaj = "Shipment creation undone!" };
        }
    }
}
