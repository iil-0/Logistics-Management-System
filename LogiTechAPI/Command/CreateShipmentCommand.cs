// Concrete Command — yeni kargo oluşturma. Projenin EN KARMAŞIK komutu;
// içinde 5 pattern bir araya gelir: Factory + Decorator + Strategy + Observer + State.
// Undo: oluşturulan kargoyu DB'den siler. Redo: Execute tekrar çağrılır.
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
    public class CreateShipmentCommand : ICargoCommand
    {
        // ScopeFactory: Komut Singleton stack'te yaşar ama her çalışmada TAZE DbContext gerekir
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly PackageFactory _factory;           // Singleton — direkt tutmak güvenli
        private readonly ShipmentRequest _request;
        private readonly int _userId;
        private readonly User _user;
        private readonly EmailSettings _emailSettings;
        private Shipment? _createdShipment;                 // Undo/Redo için — oluşturulan kargonun snapshot'ı

        public string CommandName => "Create Shipment";

        public CreateShipmentCommand(
            IServiceScopeFactory scopeFactory,
            PackageFactory factory,
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

        public async Task<CommandResult> Execute()
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var shipmentService = scope.ServiceProvider.GetRequiredService<ShipmentService>();

                // REDO yolu: daha önce oluşturulup Undo'da silinen kargoyu aynı TrackingNo ile geri yükle
                if (_createdShipment != null)
                {
                    _createdShipment.Id = 0;                                        // PK çakışmasın diye sıfırla
                    foreach (var h in _createdShipment.StatusHistory) h.Id = 0;

                    shipmentService.AddObserver(_createdShipment.TrackingNo, new NotificationObserver());
                    if (!string.IsNullOrWhiteSpace(_user.Email))
                    {
                        shipmentService.AddObserver(
                            _createdShipment.TrackingNo,
                            new EmailObserver(_user.Email, _createdShipment.TrackingNo, _emailSettings));
                    }

                    await shipmentService.AddShipment(_createdShipment);

                    shipmentService.NotifyObservers(
                        _createdShipment.TrackingNo,
                        $"Your shipment {_createdShipment.TrackingNo} has been re-created. Total: ₺{_createdShipment.TotalPrice}.",
                        "Order Received");

                    return new CommandResult { Success = true, Message = "Shipment re-created!", Shipment = _createdShipment };
                }

                // 1. FACTORY — string → IPackage somut nesnesi
                IPackage package = _factory.CreatePackage(_request.PackageType);

                // 2. DECORATOR — istenen ek hizmetler paketi sırayla sarmalar (zincir)
                if (_request.Extras != null)
                {
                    foreach (var extra in _request.Extras)
                    {
                        if (extra == "Insurance") package = new InsuranceDecorator(package);
                        if (extra == "FastDelivery") package = new FastDeliveryDecorator(package);
                    }
                }

                // 3. STRATEGY — taşıma yöntemine göre ek maliyet hesabı
                ITransportStrategy strategy = TransportStrategyFactory.GetStrategy(_request.TransportMethod);
                decimal basePrice = package.CalculatePrice();                       // Decorator zinciri sonrası fiyat
                decimal extraCost = strategy.CalculateExtraCost(basePrice);
                decimal finalPrice = basePrice + extraCost;

                // 4. MODEL — DB'ye yazılacak Shipment entity'si
                var shipment = new Shipment
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
                    PackageType = package.Name,
                    Extras = _request.Extras ?? new List<string>(),
                    TransportMethod = strategy.Name ?? string.Empty,
                    TotalPrice = finalPrice,
                    Notes = _request.Notes ?? string.Empty
                };

                // 5. OBSERVER — bildirim gözlemcilerini kaydet (e-posta + log)
                shipmentService.AddObserver(shipment.TrackingNo, new NotificationObserver());
                if (!string.IsNullOrWhiteSpace(_user.Email))
                {
                    shipmentService.AddObserver(
                        shipment.TrackingNo,
                        new EmailObserver(_user.Email, shipment.TrackingNo, _emailSettings));
                }

                // 6. STATE — ilk durum atanır + history başlatılır
                shipment.Status = "Order Received";
                shipment.StatusHistory.Add(new StatusHistory
                {
                    Status = shipment.Status,
                    Message = "Shipment created successfully.",
                    Date = DateTime.UtcNow
                });

                // 7. SAVE — DB'ye yaz, Undo için referansı sakla
                await shipmentService.AddShipment(shipment);
                _createdShipment = shipment;

                // 8. NOTIFY — observer'ları tetikle (kullanıcıya mail gider)
                shipmentService.NotifyObservers(
                    shipment.TrackingNo,
                    $"Your shipment {shipment.TrackingNo} has been created successfully. Total: ₺{shipment.TotalPrice}.",
                    "Order Received");

                return new CommandResult { Success = true, Message = "Shipment created successfully!", Shipment = shipment };
            }
            catch (Exception ex)
            {
                var innerMsg = ex.InnerException != null ? " | Inner: " + ex.InnerException.Message : "";
                return new CommandResult { Success = false, Message = "Error: " + ex.Message + innerMsg };
            }
        }

        // Undo — oluşturulmuş gönderiyi sil, observer'ları temizle
        public async Task<CommandResult> Undo()
        {
            if (_createdShipment == null)
                return new CommandResult { Success = false, Message = "Nothing to undo!" };

            using var scope = _scopeFactory.CreateScope();
            var shipmentService = scope.ServiceProvider.GetRequiredService<ShipmentService>();

            shipmentService.RemoveObservers(_createdShipment.TrackingNo);            // Redo'da çift kayıt olmasın
            await shipmentService.DeleteShipment(_createdShipment.TrackingNo);
            return new CommandResult { Success = true, Message = "Shipment creation undone!" };
        }
    }
}
