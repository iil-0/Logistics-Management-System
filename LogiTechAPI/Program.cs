using LogiTechAPI.Factory;

var builder = WebApplication.CreateBuilder(args);

// ─── Servisler ────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddSingleton<PaketFactory>();

// ─── CORS: React (localhost:3000) için ───────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy.WithOrigins(
                    "http://localhost:3000",
                    "http://localhost:5173"
               )
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ─── Middleware Pipeline ──────────────────────────────────────────────────────
// app.UseHttpsRedirection(); // Geliştirmede kapalı – CORS'u bozmamak için
app.UseCors("ReactPolicy");
app.UseAuthorization();
app.MapControllers();

app.Run();
