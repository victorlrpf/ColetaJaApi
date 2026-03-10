using System.Security.Claims;
using ColetaJaApi.Data;
using ColetaJaApi.DTOs;
using ColetaJaApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ColetaJaApi.Controllers
{
    [Authorize(Roles = "Laboratorio")]
    [Route("api/[controller]")]
    [ApiController]
    public class LaboratoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LaboratoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("pending-exams")]
        public async Task<ActionResult<IEnumerable<ExamRequestResponse>>> GetPendingExams()
        {
            var exams = await _context.ExamRequests
                .Include(e => e.ExamType)
                .Include(e => e.Address)
                .Where(e => e.Status == ExamStatus.Coletado || e.Status == ExamStatus.NoLaboratorio)
                .OrderByDescending(e => e.CreatedAt)
                .Select(e => new ExamRequestResponse
                {
                    Id = e.Id,
                    ExamType = e.ExamType!.Name,
                    Status = e.Status.ToString(),
                    Address = $"{e.Address!.Street}, {e.Address.Number}",
                    CreatedAt = e.CreatedAt
                })
                .ToListAsync();

            return Ok(exams);
        }

        [HttpPost("receive/{id}")]
        public async Task<IActionResult> ReceiveExam(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var exam = await _context.ExamRequests.FindAsync(id);

            if (exam == null) return NotFound();
            if (exam.Status != ExamStatus.Coletado) return BadRequest("O exame ainda não foi coletado.");

            exam.LaboratoryId = userId;
            exam.Status = ExamStatus.NoLaboratorio;

            await _context.SaveChangesAsync();
            return Ok("Exame recebido pelo laboratório.");
        }

        [HttpPost("insert-result/{id}")]
        public async Task<IActionResult> InsertResult(int id, [FromBody] string result)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var exam = await _context.ExamRequests.FindAsync(id);

            if (exam == null) return NotFound();
            if (exam.LaboratoryId != userId) return Forbid();

            exam.Result = result;
            exam.Status = ExamStatus.Finalizado;
            exam.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok("Resultado inserido e exame finalizado.");
        }
    }
}
