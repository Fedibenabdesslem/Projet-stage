using GestionProduit.Application.Interfaces;
using GestionProduit.Application.Services;
using GestionProduit.Domain.Interfaces;
using GestionProduit.Infrastructure.Data;
using GestionProduit.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Connexion PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Injection des dépendances pour Produits
builder.Services.AddScoped<IProduitRepository, ProduitRepository>();
builder.Services.AddScoped<IProduitService, ProduitService>();

// Injection du UserRepository et AuthService pour l'authentification
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Ajouter les services API + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuration JWT
var keyString = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured");
var key = Encoding.UTF8.GetBytes(keyString);


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],

        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Configuration CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Middleware Swagger en mode développement
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Utiliser CORS avant tout middleware qui traite les requêtes HTTP
app.UseCors("AllowAll");

app.UseHttpsRedirection();

// IMPORTANT : Authentification AVANT Autorisation
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
