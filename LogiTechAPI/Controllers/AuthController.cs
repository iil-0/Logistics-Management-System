// /api/auth altındaki kimlik doğrulama endpoint'leri.
// Cookie-based auth + Claims kullanır. Pattern içermez — düz katmanlı mimari.
// Diğer Controller (KargoController) tasarım desenlerini orkestral eder.
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using LogiTechAPI.Services;
using LogiTechAPI.DTOs.Requests;
using LogiTechAPI.DTOs.Responses;

namespace LogiTechAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;

        public AuthController(UserService userService) => _userService = userService;

        // POST /api/auth/register — Yeni hesap + otomatik giriş
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.Phone))
            {
                return BadRequest(new { message = "First name, email, password and phone number are required." });
            }

            var user = await _userService.Register(
                request.FirstName, request.LastName, request.Email,
                request.Phone, request.Password);

            // null dönüş = email zaten kayıtlı (UserService kontrolü)
            if (user == null)
                return BadRequest(new { message = "This email address is already registered." });

            await SignInUser(user);                                          // Cookie üret → otomatik login
            return Ok(ToResponse(user));
        }

        // POST /api/auth/login — Email + şifre ile giriş
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Email and password are required." });
            }

            var user = await _userService.Login(request.Email, request.Password);
            // Aynı mesaj — email var/yok bilgisini saldırgana sızdırmamak için
            if (user == null)
                return Unauthorized(new { message = "Invalid email or password." });

            await SignInUser(user);
            return Ok(ToResponse(user));
        }

        // POST /api/auth/logout — Cookie'yi temizle
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { message = "Logged out successfully." });
        }

        // GET /api/auth/me — Mevcut oturum kullanıcısı (cookie validation)
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return Unauthorized(new { message = "Session not found." });

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Invalid session." });

            var user = await _userService.GetById(userId);
            if (user == null)
                return Unauthorized(new { message = "User not found." });

            return Ok(ToResponse(user));
        }

        // Cookie üret + Response'a Set-Cookie header'ı koy
        private async Task SignInUser(Models.User user)
        {
            // Claim'ler kullanıcıyı tanımlayan key-value çiftleri; cookie içinde şifreli taşınır
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)                        // RBAC için [Authorize(Roles="...")] okur
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme,
                ClaimTypes.Name, ClaimTypes.Role);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,                                     // Tarayıcı kapansa da kalır
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24)
                });
        }

        // PasswordHash sızdırılmaz — User entity'sini direkt dönmek yerine UserResponse kullanılır
        private static UserResponse ToResponse(Models.User u) => new()
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            Phone = u.Phone,
            Role = u.Role
        };
    }
}
