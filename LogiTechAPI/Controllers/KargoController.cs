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
        /// Gönderi oluştur (Command Pattern kullanır)
        /// POST /api/kargo/gonderi-olustur
        /// </summary>
        [Authorize]
        [HttpPost("gonderi-olustur")]
        public IActionResult GonderiOlustur([FromBody] GonderiRequest request)
        {
            if (request == null)
                return BadRequest(new { mesaj = "Geçersiz istek verisi." });

            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new { mesaj = "Oturum bulunamadı." });

            var user = _userService.GetById(userId.Value);
            if (user == null)
                return Unauthorized(new { mesaj = "Kullanıcı bulunamadı." });

            // Command Pattern — Gönderi oluşturma komutunu çalıştır
            var invoker = new KargoCommandInvoker();
            var command = new GonderiOlusturCommand(
                _factory, _gonderiService, request, userId.Value, user);

            var sonuc = invoker.Calistir(command);

            if (!sonuc.Basarili)
                return BadRequest(new { mesaj = sonuc.Mesaj });

            return Ok(MapGonderiResponse(sonuc.Gonderi!));
        }

        /// <summary>
        /// Kullanıcının gönderilerini listele
        /// GET /api/kargo/gonderilerim
        /// </summary>
        [Authorize]
        [HttpGet("gonderilerim")]
        public IActionResult Gonderilerim()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new { mesaj = "Oturum bulunamadı." });

            var gonderiler = _gonderiService.KullaniciGonderileri(userId.Value);
            var response = gonderiler.Select(MapGonderiResponse).ToList();

            return Ok(response);
        }

        /// <summary>
        /// Takip numarası ile gönderi sorgula
        /// GET /api/kargo/takip/{takipNo}
        /// </summary>
        [HttpGet("takip/{takipNo}")]
        public IActionResult Takip(string takipNo)
        {
            var gonderi = _gonderiService.TakipNoIleBul(takipNo);
            if (gonderi == null)
                return NotFound(new { mesaj = "Bu takip numarasına ait gönderi bulunamadı." });

            return Ok(MapGonderiResponse(gonderi));
        }

        /// <summary>
        /// Gönderi durumunu ilerlet (Command Pattern)
        /// POST /api/kargo/durum-guncelle/{takipNo}
        /// </summary>
        [Authorize]
        [HttpPost("durum-guncelle/{takipNo}")]
        public IActionResult DurumGuncelle(string takipNo)
        {
            var invoker = new KargoCommandInvoker();
            var command = new DurumGuncelleCommand(_gonderiService, takipNo);
            var sonuc = invoker.Calistir(command);

            if (!sonuc.Basarili)
                return BadRequest(new { mesaj = sonuc.Mesaj });

            return Ok(MapGonderiResponse(sonuc.Gonderi!));
        }

        /// <summary>
        /// Gönderiyi iptal et (Command Pattern)
        /// POST /api/kargo/iptal/{takipNo}
        /// </summary>
        [Authorize]
        [HttpPost("iptal/{takipNo}")]
        public IActionResult GonderiIptal(string takipNo)
        {
            var invoker = new KargoCommandInvoker();
            var command = new GonderiIptalCommand(_gonderiService, takipNo);
            var sonuc = invoker.Calistir(command);

            if (!sonuc.Basarili)
                return BadRequest(new { mesaj = sonuc.Mesaj });

            return Ok(MapGonderiResponse(sonuc.Gonderi!));
        }

        /// <summary>
        /// API sağlık kontrolü
        /// GET /api/kargo/saglik
        /// </summary>
        [HttpGet("saglik")]
        public IActionResult SaglikKontrol()
        {
            return Ok(new
            {
                durum = "Çalışıyor ✅",
                zaman = DateTime.Now,
                versiyon = "2.0.0"
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
