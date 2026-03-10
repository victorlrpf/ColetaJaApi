using System.Security.Claims;
using ColetaJaApi.Data;
using ColetaJaApi.DTOs;
using ColetaJaApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ColetaJaApi.Controllers
{
    [Authorize(Roles = "Coletador")]
    [Route("api/[controller]")]
    [ApiController]
    public class CollectorController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CollectorController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("available-requests")]
        public async Task<ActionResult<IEnumerable<ExamRequestResponse>>> GetAvailableRequests()
        {
            var exams = await _context.ExamRequests
                .Include(e => e.ExamType)
                .Include(e => e.Address)
                .Where(e => e.Status == ExamStatus.Pendente)
                .OrderByDescending(e => e.CreatedAt)
                .Select(e => new ExamRequestResponse
                {
                    Id = e.Id,
                    ExamType = e.ExamType!.Name,
                    Status = e.Status.ToString(),
                    Address = $"{e.Address!.Street}, {e.Address.Number}, {e.Address.Neighborhood}, {e.Address.City}",
                    CreatedAt = e.CreatedAt
                })
                .ToListAsync();

            return Ok(exams);
        }

        [HttpPost("accept/{id}")]
        public async Task<IActionResult> AcceptRequest(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var exam = await _context.ExamRequests.FindAsync(id);

            if (exam == null) return NotFound();
            if (exam.Status != ExamStatus.Pendente) return BadRequest("Esta solicitação já foi aceita ou cancelada.");

            exam.CollectorId = userId;
            exam.Status = ExamStatus.AceitoPeloColetador;

            await _context.SaveChangesAsync();
            return Ok("Solicitação aceita com sucesso.");
        }

        [HttpPost("update-status/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] ExamStatus newStatus)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var exam = await _context.ExamRequests.FindAsync(id);

            if (exam == null) return NotFound();
            if (exam.CollectorId != userId) return Forbid();

            if (newStatus == ExamStatus.Coletado)
            {
                exam.CollectedAt = DateTime.UtcNow;
            }

            exam.Status = newStatus;
            await _context.SaveChangesAsync();

            return Ok($"Status atualizado para {newStatus}.");
        }

        [HttpGet("my-collections")]
        public async Task<ActionResult<IEnumerable<ExamRequestResponse>>> GetMyCollections()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            var exams = await _context.ExamRequests
                .Include(e => e.ExamType)
                .Include(e => e.Address)
                .Where(e => e.CollectorId == userId)
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
    }
}
