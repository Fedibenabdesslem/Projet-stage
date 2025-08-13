using GestionProduit.Application.DTOs;
using GestionProduit.Application.Interfaces;
using GestionProduit.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Threading.Tasks;

namespace GestionProduit.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;

        // Adresse email de l'admin (à mettre en config plus tard)
        private const string AdminEmail = "admin@example.com";

        public AuthController(IAuthService authService, IEmailService emailService)
        {
            _authService = authService;
            _emailService = emailService;
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequestDto dto)
        {
            try
            {
                var result = await _authService.SignInAsync(dto.Email, dto.Password);

                if (!result.IsSuccess)
                {
                    return Unauthorized(new { Message = result.ErrorMessage });
                }

                // Vérifier si c'est la première connexion de l'utilisateur
                if (result.IsFirstLogin && result.Role == "default")
                {
                    // Envoyer un email à l'admin
                    await _emailService.SendEmailAsync(
                        AdminEmail,
                        "Nouvel utilisateur en attente de rôle",
                        $"Un nouvel utilisateur vient de se connecter pour la première fois :\n" +
                        $"- Email : {dto.Email}\n" +
                        $"- Date : {DateTime.Now}\n\n" +
                        $"Veuillez lui attribuer un rôle."
                    );
                }

                return Ok(new { Token = result.Token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erreur serveur", Detail = ex.Message });
            }
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] RegisterDto dto)
        {
            try
            {
                var result = await _authService.RegisterAsync(dto.Username, dto.Email, dto.Password);

                if (!result.IsSuccess)
                {
                    return BadRequest(new { Message = result.ErrorMessage });
                }

                return Ok(new { Message = "Utilisateur créé avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erreur serveur", Detail = ex.Message });
            }
        }
    }
}
