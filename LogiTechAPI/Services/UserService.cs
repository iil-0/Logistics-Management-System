using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using LogiTechAPI.Models;

namespace LogiTechAPI.Services
{
    public class UserService
    {
        private readonly ConcurrentDictionary<int, User> _users = new();
        private int _nextId = 1;

        public User? Kayit(string ad, string soyad, string email, string telefon, string sifre)
        {
            // E-posta zaten kayıtlı mı?
            if (_users.Values.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
                return null;

            var user = new User
            {
                Id = _nextId++,
                Ad = ad,
                Soyad = soyad,
                Email = email,
                Telefon = telefon,
                PasswordHash = HashPassword(sifre),
                KayitTarihi = DateTime.Now
            };

            _users.TryAdd(user.Id, user);
            return user;
        }

        public User? Giris(string email, string sifre)
        {
            var user = _users.Values.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (user == null) return null;
            if (user.PasswordHash != HashPassword(sifre)) return null;

            return user;
        }

        public User? GetById(int id)
        {
            _users.TryGetValue(id, out var user);
            return user;
        }

        private static string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
