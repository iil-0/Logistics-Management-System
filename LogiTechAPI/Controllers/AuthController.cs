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
        /// New user registration
        /// POST /api/auth/register
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] KayitRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Ad) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Sifre))
            {
                return BadRequest(new { mesaj = "Name, email and password are required." });
            }

            var user = _userService.Kayit(
                request.Ad, request.Soyad, request.Email,
                request.Telefon, request.Sifre);

            if (user == null)
                return BadRequest(new { mesaj = "This email address is already registered." });

            // Auto sign in
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
        /// User login
        /// POST /api/auth/login
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] GirisRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Sifre))
            {
                return BadRequest(new { mesaj = "Email and password are required." });
            }

            var user = _userService.Giris(request.Email, request.Sifre);
            if (user == null)
                return Unauthorized(new { mesaj = "Invalid email or password." });

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
        /// Logout
        /// POST /api/auth/logout
        /// </summary>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { mesaj = "Logged out successfully." });
        }

        /// <summary>
        /// Current user info (cookie validation)
        /// GET /api/auth/me
        /// </summary>
        [HttpGet("me")]
        public IActionResult Me()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return Unauthorized(new { mesaj = "Session not found." });

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { mesaj = "Invalid session." });

            var user = _userService.GetById(userId);
            if (user == null)
                return Unauthorized(new { mesaj = "User not found." });

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
