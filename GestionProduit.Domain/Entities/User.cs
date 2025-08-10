namespace GestionProduit.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "default";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Username { get; set; } = string.Empty;

    }
}
