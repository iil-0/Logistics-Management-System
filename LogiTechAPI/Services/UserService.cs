// Kullanıcı iş mantığı servisi. Controller ile DB arasındaki katman.
// Pattern içermez — düz katmanlı mimari (Controller → Service → DbContext).
using LogiTechAPI.Data;
using LogiTechAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace LogiTechAPI.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context) => _context = context;

        // Kayıt: email çakışması yoksa hash'le ve INSERT
        public async Task<User?> Register(string firstName, string lastName, string email, string phone, string password)
        {
            // AnyAsync: sadece var/yok döner, tüm satırı çekmez → hafif sorgu
            if (await _context.Users.AnyAsync(u => u.Email == email))
                return null;                              // Email zaten kayıtlı

            var user = new User
            {
                FirstName = firstName ?? string.Empty,
                LastName = lastName ?? string.Empty,
                Email = email ?? string.Empty,
                Phone = phone ?? string.Empty,
                PasswordHash = HashPassword(password),    // Düz şifre ASLA saklanmaz
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // Giriş: email ile bul, hash karşılaştır
        public async Task<User?> Login(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || user.PasswordHash != HashPassword(password))
                return null;                              // null = "user yok VEYA şifre yanlış" (saldırgana sızıntı yok)

            return user;
        }

        // PK ile arama (önce DbContext cache'ine bakar)
        public async Task<User?> GetById(int id) => await _context.Users.FindAsync(id);

        // SHA256 + Base64. Production için BCrypt + salt önerilir.
        private static string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
