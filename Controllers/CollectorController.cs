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

        [HttpPost("update-location")]
        public async Task<IActionResult> UpdateLocation([FromBody] LocationUpdate location)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            user.Latitude = location.Latitude;
            user.Longitude = location.Longitude;
            await _context.SaveChangesAsync();

            return Ok("Localização atualizada.");
        }

        [HttpGet("available-requests")]
        public async Task<ActionResult<IEnumerable<ExamRequestResponse>>> GetAvailableRequests([FromQuery] double? lat, [FromQuery] double? lon, [FromQuery] double radiusInKm = 10)
        {
            var query = _context.ExamRequests
                .Include(e => e.ExamType)
                .Include(e => e.Address)
                .Where(e => e.Status == ExamStatus.Pendente);

            var exams = await query.ToListAsync();

            var result = exams
                .Select(e => new
                {
                    Exam = e,
                    Distance = (lat.HasValue && lon.HasValue && e.Address?.Latitude.HasValue == true && e.Address?.Longitude.HasValue == true)
                        ? CalculateDistance(lat.Value, lon.Value, e.Address.Latitude.Value, e.Address.Longitude.Value)
                        : (double?)null
                })
                .Where(x => !lat.HasValue || x.Distance <= radiusInKm)
                .OrderBy(x => x.Distance ?? double.MaxValue)
                .Select(x => new ExamRequestResponse
                {
                    Id = x.Exam.Id,
                    ExamType = x.Exam.ExamType!.Name,
                    Status = x.Exam.Status.ToString(),
                    Address = $"{x.Exam.Address!.Street}, {x.Exam.Address.Number}, {x.Exam.Address.Neighborhood}, {x.Exam.Address.City}",
                    CreatedAt = x.Exam.CreatedAt
                })
                .ToList();

            return Ok(result);
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; // Raio da Terra em km
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double deg) => deg * (Math.PI / 180);

        public class LocationUpdate
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
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
