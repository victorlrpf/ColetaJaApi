using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ColetaJaApi.Models
{
    public class Address
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Street { get; set; } = string.Empty;

        [Required]
        public string Number { get; set; } = string.Empty;

        public string Complement { get; set; } = string.Empty;

        [Required]
        public string Neighborhood { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string State { get; set; } = string.Empty;

        [Required]
        public string ZipCode { get; set; } = string.Empty;

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
