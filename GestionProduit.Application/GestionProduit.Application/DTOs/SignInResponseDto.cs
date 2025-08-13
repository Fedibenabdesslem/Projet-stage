namespace GestionProduit.Application.DTOs
{
    public class SignInResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsFirstLogin { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
