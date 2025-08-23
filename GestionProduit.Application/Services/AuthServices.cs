using GestionProduit.Application.DTOs; 
using GestionProduit.Application.Interfaces;
using GestionProduit.Domain.Entities;
using GestionProduit.Domain.Interfaces;
using GestionProduit.Infrastructure.Interfaces;
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
        private readonly IEmailService _emailService;

        public AuthService(IUserRepository userRepository, IConfiguration configuration, IEmailService emailService)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<SignInResponseDto> SignInAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null || !VerifyPassword(password, user.PasswordHash))
            {
                return new SignInResponseDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Email ou mot de passe incorrect."
                };
            }

            bool isFirstLogin = user.LastLoginAt == null;

            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            var token = GenerateJwtToken(user);

            return new SignInResponseDto
            {
                IsSuccess = true,
                Token = token,
                Role = user.Role,
                IsFirstLogin = isFirstLogin
            };
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> RegisterAsync(string username, string email, string password)
        {
            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
                return (false, "Cet email est déjà utilisé.");

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = email,
                PasswordHash = HashPassword(password),
                Role = "default",
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = null
            };

            await _userRepository.AddAsync(newUser);

            // Email à l'admin
            var adminEmail = _configuration["EmailSettings:AdminEmail"];
            if (!string.IsNullOrEmpty(adminEmail))
            {
                var subject = "Nouvel utilisateur inscrit";
                var body = $"Un nouvel utilisateur s'est inscrit : {username} ({email}).";
                await _emailService.SendEmailAsync(adminEmail, subject, body);
            }

            return (true, null);
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                  new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key not configured"))
            );
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
