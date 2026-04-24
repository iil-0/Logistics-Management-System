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

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> Register(string firstName, string lastName, string email, string phone, string password)
        {
            // Email check
            if (await _context.Users.AnyAsync(u => u.Email == email))
                return null;

            var user = new User
            {
                FirstName = firstName ?? string.Empty,
                LastName = lastName ?? string.Empty,
                Email = email ?? string.Empty,
                Phone = phone ?? string.Empty,
                PasswordHash = HashPassword(password),
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> Login(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || user.PasswordHash != HashPassword(password))
                return null;

            return user;
        }

        public async Task<User?> GetById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        private static string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
