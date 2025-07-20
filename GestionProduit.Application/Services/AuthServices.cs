using GestionProduit.Application.Interfaces;
using GestionProduit.Domain.Entities;
using GestionProduit.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GestionProduit.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<(bool IsSuccess, string Token, string ErrorMessage)> SignInAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
            {
                // Création automatique à la 1ère connexion avec rôle 'default'
                user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    PasswordHash = HashPassword(password),
                    Role = "default",
                    CreatedAt = DateTime.UtcNow
                };

                await _userRepository.AddAsync(user);
            }
            else
            {
                if (!VerifyPassword(password, user.PasswordHash))
                {
                    return (false, null, "Mot de passe incorrect.");
                }
            }

            var token = GenerateJwtToken(user);
            return (true, token, null);
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Simple hash - à remplacer par BCrypt ou autre pour plus de sécurité en production
        private string HashPassword(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(bytes);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            var hash = HashPassword(password);
            return hash == storedHash;
        }
    }
}
