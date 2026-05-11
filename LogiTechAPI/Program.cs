// Uygulamanın giriş noktası. Tüm pattern'lerin orkestratörlerini DI'a kaydeder:
// PaketFactory (Factory), ObserverRegistry (Observer Subject), KargoCommandInvoker
// (Command Invoker). Cookie auth, CORS ve DB bağlantısı da burada kurulur.

using Microsoft.AspNetCore.Authentication.Cookies;
using LogiTechAPI.Factory;
using LogiTechAPI.Services;
using LogiTechAPI.Command;
using Microsoft.EntityFrameworkCore;
using LogiTechAPI.Data;
using LogiTechAPI.Settings;

var builder = WebApplication.CreateBuilder(args);

// ─── Pattern aktörleri DI'a kaydedilir ───────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddSingleton<PaketFactory>();              // Factory: tek örnek yeterli, durum yok
builder.Services.AddSingleton<ObserverRegistry>();          // Observer Subject: tüm istekler paylaşır
builder.Services.AddSingleton<KargoCommandInvoker>();       // Command Invoker: per-user undo/redo stack
builder.Services.AddScoped<UserService>();                  // Scoped: DbContext kullanıyor
builder.Services.AddScoped<GonderiService>();               // Scoped: DbContext kullanıyor (Command Pattern'de RECEIVER)
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ─── Cookie Authentication ───────────────────────────────────────────────────
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "LogiTech.Auth";
        options.Cookie.HttpOnly = true;                     // XSS koruması
        options.Cookie.SameSite = SameSiteMode.Lax;         // CSRF koruması
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = true;

        // API projesi: redirect yerine 401/403 status kodu dön
        options.Events.OnRedirectToLogin = ctx => { ctx.Response.StatusCode = 401; return Task.CompletedTask; };
        options.Events.OnRedirectToAccessDenied = ctx => { ctx.Response.StatusCode = 403; return Task.CompletedTask; };
    });

builder.Services.AddAuthorization();

// ─── CORS: React frontend için ───────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173", "http://localhost:5174")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();                          // Cookie taşıma izni
    });
});

var app = builder.Build();

// ─── Middleware pipeline (sıra önemli) ───────────────────────────────────────
app.UseCors("ReactPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
