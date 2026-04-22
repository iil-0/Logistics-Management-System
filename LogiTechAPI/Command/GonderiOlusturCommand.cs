using LogiTechAPI.Models;
using LogiTechAPI.Factory;
using LogiTechAPI.Decorator;
using LogiTechAPI.Strategy;
using LogiTechAPI.Observer;
using LogiTechAPI.State;
using LogiTechAPI.Services;
using LogiTechAPI.DTOs.Requests;

namespace LogiTechAPI.Command
{
    public class GonderiOlusturCommand : IKargoCommand
    {
        private readonly PaketFactory _factory;
        private readonly GonderiService _gonderiService;
        private readonly ShipmentRequest _request;
        private readonly int _userId;
        private readonly User _user;
        private Gonderi? _createdShipment;

        public string KomutAdi => "Create Shipment";

        public GonderiOlusturCommand(
            PaketFactory factory,
            GonderiService gonderiService,
            ShipmentRequest request,
            int userId,
            User user)
        {
            _factory = factory;
            _gonderiService = gonderiService;
            _request = request;
            _userId = userId;
            _user = user;
        }

        public async Task<KomutSonuc> Execute()
        {
            try
            {
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
                    TrackingNo = "LT-" + DateTime.Now.ToString("yyyyMMdd") + "-" + new Random().Next(1000, 9999),
                    SenderName = $"{_user.FirstName} {_user.LastName}",
                    SenderEmail = _user.Email,
                    SenderPhone = _user.Phone,
                    ReceiverName = _request.ReceiverName,
                    ReceiverAddress = _request.ReceiverAddress,
                    ReceiverPhone = _request.ReceiverPhone,
                    ReceiverCity = _request.ReceiverCity,
                    PackageType = paket.Name,
                    Extras = _request.Extras ?? new List<string>(),
                    TransportMethod = strategy.Name,
                    TotalPrice = finalPrice,
                    Notes = _request.Notes ?? string.Empty
                };

                // 5. State & Observer
                var observer = new NotificationObserver();
                _gonderiService.AddObserver(shipment.TrackingNo, observer);

                shipment.Status = "Order Received";
                shipment.StatusHistory.Add(new StatusHistory 
                { 
                    Status = shipment.Status, 
                    Message = "Shipment created successfully.",
                    Date = DateTime.Now
                });

                // 6. Save
                await _gonderiService.AddShipment(shipment);
                _createdShipment = shipment;

                return new KomutSonuc { Basarili = true, Mesaj = "Shipment created successfully!", Gonderi = shipment };
            }
            catch (Exception ex)
            {
                return new KomutSonuc { Basarili = false, Mesaj = "Error: " + ex.Message };
            }
        }

        public async Task<KomutSonuc> Undo()
        {
            if (_createdShipment != null)
            {
                await _gonderiService.DeleteShipment(_createdShipment.TrackingNo);
                return new KomutSonuc { Basarili = true, Mesaj = "Shipment creation undone!" };
            }
            return new KomutSonuc { Basarili = false, Mesaj = "Nothing to undo!" };
        }
    }
}
