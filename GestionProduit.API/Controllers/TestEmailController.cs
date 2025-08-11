using GestionProduit.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GestionProduit.API.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestEmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public TestEmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        // Recommandé : méthode POST et email passé en query ou body
        [HttpPost("send-email")]
        public async Task<IActionResult> SendTestEmail([FromQuery] string toEmail)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
            {
                return BadRequest("Le paramètre 'toEmail' est requis et ne peut pas être vide.");
            }

            await _emailService.SendEmailAsync(toEmail, "Test Email", "Ceci est un email de test.");
            return Ok("Email envoyé !");
        }
    }
}
