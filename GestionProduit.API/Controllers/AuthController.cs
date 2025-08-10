
using GestionProduit.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using GestionProduit.Application.DTOs;

using System;
using System.Threading.Tasks;

namespace GestionProduit.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
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