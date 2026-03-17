using ColetaJaApi.Data;
using ColetaJaApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ColetaJaApi.Controllers
{
    [Authorize(Roles = "Laboratorio")] // Inicialmente, laboratórios podem gerenciar exames
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("exam-types")]
        public async Task<ActionResult<IEnumerable<ExamType>>> GetExamTypes()
        {
            return await _context.ExamTypes.ToListAsync();
        }

        [HttpPost("exam-types")]
        public async Task<ActionResult<ExamType>> CreateExamType(ExamType examType)
        {
            _context.ExamTypes.Add(examType);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetExamTypes), new { id = examType.Id }, examType);
        }

        [HttpPut("exam-types/{id}")]
        public async Task<IActionResult> UpdateExamType(int id, ExamType examType)
        {
            if (id != examType.Id) return BadRequest();

            _context.Entry(examType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExamTypeExists(id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("exam-types/{id}")]
        public async Task<IActionResult> DeleteExamType(int id)
        {
            var examType = await _context.ExamTypes.FindAsync(id);
            if (examType == null) return NotFound();

            _context.ExamTypes.Remove(examType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExamTypeExists(int id)
        {
            return _context.ExamTypes.Any(e => e.Id == id);
        }
    }
}
