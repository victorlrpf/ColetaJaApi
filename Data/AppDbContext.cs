using Microsoft.EntityFrameworkCore;
using ColetaJaApi.Models;

namespace ColetaJaApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<ExamType> ExamTypes { get; set; }
        public DbSet<ExamRequest> ExamRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações adicionais se necessário
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Relacionamentos para ExamRequest
            modelBuilder.Entity<ExamRequest>()
                .HasOne(e => e.Patient)
                .WithMany(u => u.ExamRequests)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExamRequest>()
                .HasOne(e => e.Collector)
                .WithMany()
                .HasForeignKey(e => e.CollectorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExamRequest>()
                .HasOne(e => e.Laboratory)
                .WithMany()
                .HasForeignKey(e => e.LaboratoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
