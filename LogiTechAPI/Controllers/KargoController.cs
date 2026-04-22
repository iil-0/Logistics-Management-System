using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using LogiTechAPI.Factory;
using LogiTechAPI.Services;
using LogiTechAPI.DTOs;
using LogiTechAPI.Command;
using LogiTechAPI.State;

namespace LogiTechAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        public IActionResult GonderiOlustur([FromBody] GonderiRequest request)
        {
            if (request == null)
                return BadRequest(new { mesaj = "Invalid request data." });

            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new { mesaj = "Session not found." });

            var user = _userService.GetById(userId.Value);
            if (user == null)
                return Unauthorized(new { mesaj = "User not found." });

            // Command Pattern — Run shipment creation command
            var invoker = new KargoCommandInvoker();
            var command = new GonderiOlusturCommand(
                _factory, _gonderiService, request, userId.Value, user);

            var sonuc = invoker.Calistir(command);

            if (!sonuc.Basarili)
                return BadRequest(new { mesaj = sonuc.Mesaj });

            return Ok(MapGonderiResponse(sonuc.Gonderi!));
        }

        /// <summary>
        /// List user shipments
        /// GET /api/cargo/my-shipments
        /// </summary>
        [Authorize]
        [HttpGet("my-shipments")]
        public IActionResult Gonderilerim()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new { mesaj = "Session not found." });

            var gonderiler = _gonderiService.KullaniciGonderileri(userId.Value);
            var response = gonderiler.Select(MapGonderiResponse).ToList();

            return Ok(response);
        }

        /// <summary>
        /// Track shipment with tracking number
        /// GET /api/cargo/track/{trackingNo}
        /// </summary>
        [HttpGet("track/{trackingNo}")]
        public IActionResult Takip(string trackingNo)
        {
            var gonderi = _gonderiService.TakipNoIleBul(trackingNo);
            if (gonderi == null)
                return NotFound(new { mesaj = "Shipment not found with this tracking number." });

            return Ok(MapGonderiResponse(gonderi));
        }

        /// <summary>
        /// Advance shipment status (Command Pattern)
        /// POST /api/cargo/update-status/{trackingNo}
        /// </summary>
        [Authorize]
        [HttpPost("update-status/{trackingNo}")]
        public IActionResult DurumGuncelle(string trackingNo)
        {
            var invoker = new KargoCommandInvoker();
            var command = new DurumGuncelleCommand(_gonderiService, trackingNo);
            var sonuc = invoker.Calistir(command);

            if (!sonuc.Basarili)
                return BadRequest(new { mesaj = sonuc.Mesaj });

            return Ok(MapGonderiResponse(sonuc.Gonderi!));
        }

        /// <summary>
        /// Cancel shipment (Command Pattern)
        /// POST /api/cargo/cancel/{trackingNo}
        /// </summary>
        [Authorize]
        [HttpPost("cancel/{trackingNo}")]
        public IActionResult GonderiIptal(string trackingNo)
        {
            var invoker = new KargoCommandInvoker();
            var command = new GonderiIptalCommand(_gonderiService, trackingNo);
            var sonuc = invoker.Calistir(command);

            if (!sonuc.Basarili)
                return BadRequest(new { mesaj = sonuc.Mesaj });

            return Ok(MapGonderiResponse(sonuc.Gonderi!));
        }

        /// <summary>
        /// API health check
        /// GET /api/cargo/health
        /// </summary>
        [HttpGet("health")]
        public IActionResult SaglikKontrol()
        {
            return Ok(new
            {
                status = "Healthy ✅",
                time = DateTime.Now,
                version = "2.0.0"
            });
        }

        // ─── Yardımcı Metotlar ────────────────────────────────────────────────

        private int? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (claim != null && int.TryParse(claim, out var id)) return id;
            return null;
        }

        private static GonderiResponse MapGonderiResponse(Models.Gonderi g)
        {
            var mevcutDurum = GonderiDurumFactory.GetDurum(g.Durum);
            return new GonderiResponse
            {
                Id = g.Id,
                TakipNo = g.TakipNo,
                GondericiAd = g.GondericiAd,
                AliciAd = g.AliciAd,
                AliciAdres = g.AliciAdres,
                AliciSehir = g.AliciSehir,
                PaketTipi = g.PaketTipi,
                Ekstralar = g.Ekstralar,
                TasimaYolu = g.TasimaYolu,
                ToplamFiyat = g.ToplamFiyat,
                Durum = g.Durum,
                IptalEdilabilir = mevcutDurum.IptalEdilabilir(),
                DurumGecmisi = g.DurumGecmisi.Select(d => new DurumGecmisiResponse
                {
                    Durum = d.Durum,
                    Tarih = d.Tarih,
                    Aciklama = d.Aciklama
                }).ToList(),
                OlusturulmaTarihi = g.OlusturulmaTarihi
            };
        }
    }
}
