// /api/cargo altındaki tüm HTTP endpoint'leri. Command pattern'in CLIENT'ı:
// HTTP isteğini komut nesnesine sarıp Invoker'a verir. State pattern'i de
// MapShipmentResponse içinde IsCancellable hesaplamak için kullanır.
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using LogiTechAPI.Factory;
using LogiTechAPI.Services;
using LogiTechAPI.Settings;
using LogiTechAPI.DTOs.Requests;
using LogiTechAPI.DTOs.Responses;
using LogiTechAPI.Command;
using LogiTechAPI.State;
using LogiTechAPI.Models;

namespace LogiTechAPI.Controllers
{
    [ApiController]
    [Route("api/cargo")]
    public class CargoController : ControllerBase
    {
        // Tüm bağımlılıklar DI ile yapıcıya enjekte edilir
        private readonly PackageFactory _factory;                // Factory: paket üretimi
        private readonly ShipmentService _shipmentService;       // RECEIVER + sorgu işleri
        private readonly UserService _userService;
        private readonly ILogger<CargoController> _logger;
        private readonly EmailSettings _emailSettings;
        private readonly CargoCommandInvoker _invoker;           // INVOKER (Singleton — per-user undo stack)
        private readonly IServiceScopeFactory _scopeFactory;     // Komutlara TAZE scope açtırır (captive dep önlemi)

        public CargoController(
            PackageFactory factory,
            ShipmentService shipmentService,
            UserService userService,
            ILogger<CargoController> logger,
            IOptions<EmailSettings> emailSettings,
            CargoCommandInvoker invoker,
            IServiceScopeFactory scopeFactory)
        {
            _factory = factory;
            _shipmentService = shipmentService;
            _userService = userService;
            _logger = logger;
            _emailSettings = emailSettings.Value;
            _invoker = invoker;
            _scopeFactory = scopeFactory;
        }

        // POST /api/cargo/create-shipment — Yeni kargo (CLIENT rolü)
        [Authorize]
        [HttpPost("create-shipment")]
        public async Task<IActionResult> CreateShipment([FromBody] ShipmentRequest request)
        {
            if (request == null)
                return BadRequest(new { message = "Invalid request data." });

            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new { message = "Session not found." });

            var user = await _userService.GetById(userId.Value);
            if (user == null)
                return Unauthorized(new { message = "User not found." });

            // CLIENT — komutu yarat, Invoker'a teslim et. Invoker bunu undo stack'ine koyar.
            var command = new CreateShipmentCommand(
                _scopeFactory, _factory, request, userId.Value, user, _emailSettings);
            var result = await _invoker.ExecuteCommand(userId.Value, command);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(MapShipmentResponse((Shipment)result.Shipment!));
        }

        // GET /api/cargo/my-shipments — Kullanıcının kendi kargoları
        [Authorize]
        [HttpGet("my-shipments")]
        public async Task<IActionResult> MyShipments()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new { message = "Session not found." });

            var shipments = await _shipmentService.GetUserShipments(userId.Value);
            return Ok(shipments.Select(MapShipmentResponse).ToList());
        }

        // GET /api/cargo/all-shipments — Tüm kargolar (sadece Admin)
        [Authorize(Roles = "Admin")]
        [HttpGet("all-shipments")]
        public async Task<IActionResult> AllShipments()
        {
            var shipments = await _shipmentService.GetAllShipments();
            return Ok(shipments.Select(MapShipmentResponse).ToList());
        }

        // GET /api/cargo/track/{trackingNo} — Anonim erişim, herkes takip edebilir
        [HttpGet("track/{trackingNo}")]
        public async Task<IActionResult> Track(string trackingNo)
        {
            var shipment = await _shipmentService.GetByTrackingNo(trackingNo);
            if (shipment == null)
                return NotFound(new { message = "Shipment not found with this tracking number." });

            return Ok(MapShipmentResponse(shipment));
        }

        // POST /api/cargo/update-status/{trackingNo} — Status ilerletme (Admin)
        [Authorize(Roles = "Admin")]
        [HttpPost("update-status/{trackingNo}")]
        public async Task<IActionResult> UpdateStatus(string trackingNo, [FromBody] string nextStatus)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new { message = "Session not found." });

            // CLIENT — UpdateStatusCommand'ı invoker'a ver
            var command = new UpdateStatusCommand(_scopeFactory, trackingNo, nextStatus);
            var result = await _invoker.ExecuteCommand(userId.Value, command);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            var shipment = await _shipmentService.GetByTrackingNo(trackingNo);
            return Ok(MapShipmentResponse(shipment!));
        }

        // POST /api/cargo/cancel/{trackingNo} — İptal (sahibi VEYA admin)
        [Authorize]
        [HttpPost("cancel/{trackingNo}")]
        public async Task<IActionResult> CancelShipment(string trackingNo)
        {
            var shipment = await _shipmentService.GetByTrackingNo(trackingNo);
            if (shipment == null)
                return NotFound(new { message = "Shipment not found." });

            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new { message = "Session not found." });
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Ownership-based authorization — rol kontrolü tek başına yeterli değil
            if (shipment.UserId != userId && userRole != "Admin")
                return Forbid();

            // CLIENT — iptal komutunu invoker'a ver
            var command = new CancelShipmentCommand(_scopeFactory, trackingNo);
            var result = await _invoker.ExecuteCommand(userId.Value, command);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            var updatedShipment = await _shipmentService.GetByTrackingNo(trackingNo);
            return Ok(MapShipmentResponse(updatedShipment!));
        }

        // POST /api/cargo/undo — Son komutu geri al (Command pattern)
        [Authorize]
        [HttpPost("undo")]
        public async Task<IActionResult> Undo()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new { message = "Session not found." });

            var result = await _invoker.UndoLastCommand(userId.Value);
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        // POST /api/cargo/redo — Undo edileni yeniden uygula
        [Authorize]
        [HttpPost("redo")]
        public async Task<IActionResult> Redo()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new { message = "Session not found." });

            var result = await _invoker.RedoLastCommand(userId.Value);
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        // GET /api/cargo/history-status — Frontend butonları için canUndo/canRedo
        [Authorize]
        [HttpGet("history-status")]
        public IActionResult HistoryStatus()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new { message = "Session not found." });

            var (canUndo, canRedo) = _invoker.GetStatus(userId.Value);
            return Ok(new { canUndo, canRedo });
        }

        // GET /api/cargo/health — Canlılık kontrolü (anonim)
        [HttpGet("health")]
        public IActionResult HealthCheck()
            => Ok(new { status = "Healthy", time = DateTime.Now, version = "3.1.0" });

        // ─── Helpers ─────────────────────────────────────────────────────────

        private int? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (claim != null && int.TryParse(claim, out var id)) return id;
            return null;
        }

        // Entity → DTO. IsCancellable State pattern üzerinden anlık hesaplanır.
        private static ShipmentResponse MapShipmentResponse(Shipment g)
        {
            var currentStatus = ShipmentStatusFactory.GetStatus(g.Status);  // STATE pattern köprüsü
            return new ShipmentResponse
            {
                Id = g.Id,
                TrackingNo = g.TrackingNo,
                SenderName = g.SenderName,
                ReceiverName = g.ReceiverName,
                ReceiverAddress = g.ReceiverAddress,
                ReceiverCity = g.ReceiverCity,
                PackageType = g.PackageType,
                Extras = g.Extras,
                TransportMethod = g.TransportMethod,
                TotalPrice = g.TotalPrice,
                Status = g.Status,
                IsCancellable = currentStatus.IsCancellable(),               // State'in iş kuralı
                StatusHistory = g.StatusHistory.Select(d => new StatusHistoryResponse
                {
                    Status = d.Status,
                    Date = d.Date,
                    Description = d.Message
                }).ToList(),
                CreatedAt = g.CreatedAt
            };
        }
    }
}
