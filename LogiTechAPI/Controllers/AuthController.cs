using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using LogiTechAPI.Services;
using LogiTechAPI.DTOs;

namespace LogiTechAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;

        public AuthController(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Yeni kullanıcı kaydı
        /// POST /api/auth/kayit
        /// </summary>
        [HttpPost("kayit")]
        public async Task<IActionResult> Kayit([FromBody] KayitRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Ad) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Sifre))
            {
                return BadRequest(new { mesaj = "Ad, e-posta ve şifre zorunludur." });
            }

            var user = _userService.Kayit(
                request.Ad, request.Soyad, request.Email,
                request.Telefon, request.Sifre);

            if (user == null)
                return BadRequest(new { mesaj = "Bu e-posta adresi zaten kayıtlı." });

            // Otomatik giriş yap
            await SignInUser(user);

            return Ok(new UserResponse
            {
                Id = user.Id,
                Ad = user.Ad,
                Soyad = user.Soyad,
                Email = user.Email,
                Telefon = user.Telefon
            });
        }

        /// <summary>
        /// Kullanıcı girişi
        /// POST /api/auth/giris
        /// </summary>
        [HttpPost("giris")]
        public async Task<IActionResult> Giris([FromBody] GirisRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Sifre))
            {
                return BadRequest(new { mesaj = "E-posta ve şifre zorunludur." });
            }

            var user = _userService.Giris(request.Email, request.Sifre);
            if (user == null)
                return Unauthorized(new { mesaj = "E-posta veya şifre hatalı." });

            await SignInUser(user);

            return Ok(new UserResponse
            {
                Id = user.Id,
                Ad = user.Ad,
                Soyad = user.Soyad,
                Email = user.Email,
                Telefon = user.Telefon
            });
        }

        /// <summary>
        /// Çıkış yap
        /// POST /api/auth/cikis
        /// </summary>
        [HttpPost("cikis")]
        public async Task<IActionResult> Cikis()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { mesaj = "Çıkış yapıldı." });
        }

        /// <summary>
        /// Mevcut kullanıcı bilgisi (cookie doğrulama)
        /// GET /api/auth/ben
        /// </summary>
        [HttpGet("ben")]
        public IActionResult MevcutKullanici()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return Unauthorized(new { mesaj = "Oturum bulunamadı." });

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { mesaj = "Oturum geçersiz." });

            var user = _userService.GetById(userId);
            if (user == null)
                return Unauthorized(new { mesaj = "Kullanıcı bulunamadı." });

            return Ok(new UserResponse
            {
                Id = user.Id,
                Ad = user.Ad,
                Soyad = user.Soyad,
                Email = user.Email,
                Telefon = user.Telefon
            });
        }

        private async Task SignInUser(Models.User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.Ad} {user.Soyad}"),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24)
                });
        }
    }
}
