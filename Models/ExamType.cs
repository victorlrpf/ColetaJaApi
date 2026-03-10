using System.ComponentModel.DataAnnotations;

namespace ColetaJaApi.Models
{
    public class ExamType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }
    }
}
