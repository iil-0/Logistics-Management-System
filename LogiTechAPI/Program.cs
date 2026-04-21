using Microsoft.AspNetCore.Authentication.Cookies;
using LogiTechAPI.Factory;
using LogiTechAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// ─── Servisler ────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddSingleton<PaketFactory>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<GonderiService>();

// ─── Cookie Authentication ───────────────────────────────────────────────────
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "LogiTech.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = true;

        // API projesi olduğu için redirect yerine 401 döndür
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = 403;
            return Task.CompletedTask;
        };
    });

// ─── CORS: React (localhost:5173 / localhost:3000) için ───────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy.WithOrigins(
                    "http://localhost:3000",
                    "http://localhost:5173",
                    "http://localhost:5174"
               )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();  // Cookie gönderimi için gerekli
    });
});

var app = builder.Build();

// ─── Middleware Pipeline ──────────────────────────────────────────────────────
app.UseCors("ReactPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
