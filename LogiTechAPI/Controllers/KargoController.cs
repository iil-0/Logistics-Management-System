using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using LogiTechAPI.Factory;
using LogiTechAPI.Services;
using LogiTechAPI.DTOs.Requests;
using LogiTechAPI.DTOs.Responses;
using LogiTechAPI.Command;
using LogiTechAPI.State;
using LogiTechAPI.Models;

namespace LogiTechAPI.Controllers
{
    [ApiController]
    [Route("api/cargo")]
    public class KargoController : ControllerBase
    {
        private readonly PaketFactory _factory;
        private readonly GonderiService _gonderiService;
        private readonly UserService _userService;
        private readonly ILogger<KargoController> _logger;

        public KargoController(
            PaketFactory factory,
            GonderiService gonderiService,
            UserService userService,
            ILogger<KargoController> logger)
        {
            _factory = factory;
            _gonderiService = gonderiService;
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Create shipment (uses Command Pattern)
        /// POST /api/cargo/create-shipment
        /// </summary>
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

            // Command Pattern — Run shipment creation command
            var invoker = new KargoCommandInvoker();
            var command = new GonderiOlusturCommand(
                _factory, _gonderiService, request, userId.Value, user);

            var result = await invoker.ExecuteCommand(command);

            if (!result.Basarili)
                return BadRequest(new { message = result.Mesaj });

            return Ok(MapShipmentResponse((Gonderi)result.Gonderi!));
        }

        /// <summary>
        /// List user shipments
        /// GET /api/cargo/my-shipments
        /// </summary>
        [Authorize]
        [HttpGet("my-shipments")]
        public async Task<IActionResult> MyShipments()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new { message = "Session not found." });

            var shipments = await _gonderiService.GetUserShipments(userId.Value);
            var response = shipments.Select(MapShipmentResponse).ToList();

            return Ok(response);
        }

        /// <summary>
        /// List ALL shipments (Admin Only)
        /// GET /api/cargo/all-shipments
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("all-shipments")]
        public async Task<IActionResult> AllShipments()
        {
            Console.WriteLine("----- ADMIN IDENTITY DEBUG START -----");
            foreach (var c in User.Claims)
            {
                Console.WriteLine($"Claim: {c.Type} = {c.Value}");
            }
            Console.WriteLine($"IsAuthenticated: {User.Identity?.IsAuthenticated}");
            Console.WriteLine("----- ADMIN IDENTITY DEBUG END -----");

            var shipments = await _gonderiService.GetAllShipments();
            var response = shipments.Select(MapShipmentResponse).ToList();

            return Ok(response);
        }

        /// <summary>
        /// Track shipment with tracking number
        /// GET /api/cargo/track/{trackingNo}
        /// </summary>
        [HttpGet("track/{trackingNo}")]
        public async Task<IActionResult> Track(string trackingNo)
        {
            var shipment = await _gonderiService.GetByTrackingNo(trackingNo);
            if (shipment == null)
                return NotFound(new { message = "Shipment not found with this tracking number." });

            return Ok(MapShipmentResponse(shipment));
        }

        /// <summary>
        /// Advance shipment status (Command Pattern) - Admin Only
        /// POST /api/cargo/update-status/{trackingNo}
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("update-status/{trackingNo}")]
        public async Task<IActionResult> UpdateStatus(string trackingNo, [FromBody] string nextStatus)
        {
            var invoker = new KargoCommandInvoker();
            var command = new DurumGuncelleCommand(_gonderiService, trackingNo, nextStatus);
            var result = await invoker.ExecuteCommand(command);

            if (!result.Basarili)
                return BadRequest(new { message = result.Mesaj });

            var shipment = await _gonderiService.GetByTrackingNo(trackingNo);
            return Ok(MapShipmentResponse(shipment!));
        }

        /// <summary>
        /// Cancel shipment (Command Pattern)
        /// POST /api/cargo/cancel/{trackingNo}
        /// </summary>
        [Authorize]
        [HttpPost("cancel/{trackingNo}")]
        public async Task<IActionResult> CancelShipment(string trackingNo)
        {
            Console.WriteLine("----- IDENTITY DEBUG START -----");
            foreach (var c in User.Claims)
            {
                Console.WriteLine($"Claim: {c.Type} = {c.Value}");
            }
            Console.WriteLine($"IsAuthenticated: {User.Identity?.IsAuthenticated}");
            Console.WriteLine("----- IDENTITY DEBUG END -----");

            Console.WriteLine($"[DEBUG] CancelShipment endpoint reached for: {trackingNo}");
            var shipment = await _gonderiService.GetByTrackingNo(trackingNo);
            if (shipment == null)
                return NotFound(new { message = "Shipment not found." });

            var userId = GetUserId();
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Check if current user is owner OR admin
            if (shipment.UserId != userId && userRole != "Admin")
                return Forbid();

            var invoker = new KargoCommandInvoker();
            var command = new GonderiIptalCommand(_gonderiService, trackingNo);
            var result = await invoker.ExecuteCommand(command);

            if (!result.Basarili)
                return BadRequest(new { message = result.Mesaj });

            var updatedShipment = await _gonderiService.GetByTrackingNo(trackingNo);
            return Ok(MapShipmentResponse(updatedShipment!));
        }

        /// <summary>
        /// API health check
        /// GET /api/cargo/health
        /// </summary>
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new
            {
                status = "Healthy ✅",
                time = DateTime.Now,
                version = "3.0.0"
            });
        }

        // ─── Helper Methods ───────────────────────────────────────────────────

        private int? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (claim != null && int.TryParse(claim, out var id)) return id;
            return null;
        }

        private static ShipmentResponse MapShipmentResponse(Gonderi g)
        {
            var currentStatus = GonderiDurumFactory.GetStatus(g.Status);
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
                IsCancellable = currentStatus.IsCancellable(),
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
