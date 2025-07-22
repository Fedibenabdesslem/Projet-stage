using GestionProduit.Application.DTOs;
using GestionProduit.Application.Interfaces;
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

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn(SignInRequestDto dto)
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

    }
}
