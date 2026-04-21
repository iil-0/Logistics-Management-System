using Microsoft.AspNetCore.Mvc;
using LogiTechAPI.Factory;
using LogiTechAPI.Decorator;
using LogiTechAPI.Strategy;
using LogiTechAPI.Observer;
using LogiTechAPI.DTOs;

namespace LogiTechAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KargoController : ControllerBase
    {
        private readonly PaketFactory _factory;
        private readonly ILogger<KargoController> _logger;

        public KargoController(PaketFactory factory, ILogger<KargoController> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        /// <summary>
        /// Kargo fiyatını ve detaylarını hesaplar
        /// POST /api/kargo/hesapla
        /// </summary>
        [HttpPost("hesapla")]
        public IActionResult HesaplaKargo([FromBody] KargoSiparisRequest request)
        {
            if (request == null)
                return BadRequest(new { mesaj = "Geçersiz istek verisi." });

            _logger.LogInformation("Kargo hesaplama isteği: {PaketTipi}, {TasimaYolu}", 
                request.PaketTipi, request.TasimaYolu);

            // ─── 1. FACTORY: Paket oluştur ───────────────────────────────────
            IPaket paket = _factory.CreatePaket(request.PaketTipi);

            var ekstraHizmetler = new List<string>();

            // ─── 2. DECORATOR: Ekstraları uygula ─────────────────────────────
            if (request.Ekstralar != null)
            {
                foreach (var extra in request.Ekstralar)
                {
                    switch (extra.ToLower())
                    {
                        case "sigorta":
                            paket = new SigortaDecorator(paket);
                            ekstraHizmetler.Add("Sigorta Güvencesi (+75₺)");
                            break;
                        case "hizliteslimat":
                            paket = new HizliTeslimatDecorator(paket);
                            ekstraHizmetler.Add("Hızlı Teslimat (+100₺)");
                            break;
                    }
                }
            }

            // ─── 3. STRATEGY: Taşıma yolu maliyetini hesapla ─────────────────
            var strateji = TasimaStratejisiFactory.GetStrateji(request.TasimaYolu);
            var temelFiyat = paket.GetFiyat();
            var ekMaliyet = strateji.HesaplaEkMaliyet(temelFiyat);
            var toplamFiyat = temelFiyat + ekMaliyet;

            // ─── 4. OBSERVER: Durum bildirimleri gönder ──────────────────────
            var kargoTakip = new KargoTakipServisi();
            var bildirimObserver = new BildirimObserver();
            var emailObserver = new EmailObserver();

            kargoTakip.Subscribe(bildirimObserver);
            kargoTakip.Subscribe(emailObserver);

            kargoTakip.Notify("Kargo siparişi sisteme alındı.", "Sipariş Alındı ✅");
            kargoTakip.Notify($"{request.TasimaYolu} güzergahı için rota planlanıyor.", "Rota Planlanıyor 🗺️");
            kargoTakip.Notify("Kargo hazırlık aşamasına geçirildi.", "Hazırlanıyor 📦");

            // Tüm bildirimleri birleştir
            var tumBildirimler = bildirimObserver.Mesajlar
                .Concat(emailObserver.Mesajlar)
                .ToList();

            // ─── 5. RESPONSE oluştur ──────────────────────────────────────────
            var response = new KargoSiparisResponse
            {
                ToplamFiyat = Math.Round(toplamFiyat, 2),
                Aciklama = paket.GetAciklama() + " | " + strateji.GetAciklama(),
                PaketTipi = paket.GetTip(),
                TasimaYolu = strateji.GetTip(),
                EkstraHizmetler = ekstraHizmetler,
                KargoDurumu = kargoTakip.GetDurum(),
                Bildirimler = tumBildirimler,
                OlusturulmaTarihi = DateTime.Now
            };

            return Ok(response);
        }

        /// <summary>
        /// Sağlık kontrolü
        /// GET /api/kargo/saglik
        /// </summary>
        [HttpGet("saglik")]
        public IActionResult SaglikKontrol()
        {
            return Ok(new
            {
                durum = "Çalışıyor ✅",
                zaman = DateTime.Now,
                versiyon = "1.0.0"
            });
        }
    }
}
