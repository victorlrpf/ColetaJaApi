using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ColetaJaApi.Models
{
    public enum ExamStatus
    {
        Pendente,
        AceitoPeloColetador,
        Coletado,
        NoLaboratorio,
        Finalizado,
        Cancelado
    }

    public class ExamRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public User? Patient { get; set; }

        public int? CollectorId { get; set; }

        [ForeignKey("CollectorId")]
        public User? Collector { get; set; }

        public int? LaboratoryId { get; set; }

        [ForeignKey("LaboratoryId")]
        public User? Laboratory { get; set; }

        [Required]
        public int ExamTypeId { get; set; }

        [ForeignKey("ExamTypeId")]
        public ExamType? ExamType { get; set; }

        [Required]
        public int AddressId { get; set; }

        [ForeignKey("AddressId")]
        public Address? Address { get; set; }

        [Required]
        public ExamStatus Status { get; set; } = ExamStatus.Pendente;

        public string? Result { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? CollectedAt { get; set; }

        public DateTime? CompletedAt { get; set; }
    }
}
