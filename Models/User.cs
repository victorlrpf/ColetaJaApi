using System.ComponentModel.DataAnnotations;

namespace ColetaJaApi.Models
{
    public enum UserType
    {
        Paciente,
        Coletador,
        Laboratorio
    }

    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public UserType Type { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relacionamentos
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public ICollection<ExamRequest> ExamRequests { get; set; } = new List<ExamRequest>();
    }
}
