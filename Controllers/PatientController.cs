using System.Security.Claims;
using ColetaJaApi.Data;
using ColetaJaApi.DTOs;
using ColetaJaApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ColetaJaApi.Controllers
{
    [Authorize(Roles = "Paciente")]
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PatientController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("address")]
        public async Task<ActionResult<Address>> AddAddress(AddressCreateRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            var address = new Address
            {
                Street = request.Street,
                Number = request.Number,
                Complement = request.Complement,
                Neighborhood = request.Neighborhood,
                City = request.City,
                State = request.State,
                ZipCode = request.ZipCode,
                UserId = userId
            };

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            return Ok(address);
        }

        [HttpGet("addresses")]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddresses()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var addresses = await _context.Addresses.Where(a => a.UserId == userId).ToListAsync();
            return Ok(addresses);
        }

        [HttpGet("exam-types")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ExamType>>> GetExamTypes()
        {
            return await _context.ExamTypes.ToListAsync();
        }

        [HttpPost("request-exam")]
        public async Task<ActionResult<ExamRequestResponse>> RequestExam(ExamRequestCreate request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var examRequest = new ExamRequest
            {
                PatientId = userId,
                ExamTypeId = request.ExamTypeId,
                AddressId = request.AddressId,
                Status = ExamStatus.Pendente
            };

            _context.ExamRequests.Add(examRequest);
            await _context.SaveChangesAsync();

            var response = await _context.ExamRequests
                .Include(e => e.ExamType)
                .Include(e => e.Address)
                .FirstOrDefaultAsync(e => e.Id == examRequest.Id);

            return Ok(new ExamRequestResponse
            {
                Id = response!.Id,
                ExamType = response.ExamType!.Name,
                Status = response.Status.ToString(),
                Address = $"{response.Address!.Street}, {response.Address.Number}",
                CreatedAt = response.CreatedAt
            });
        }

        [HttpGet("my-exams")]
        public async Task<ActionResult<IEnumerable<ExamRequestResponse>>> GetMyExams()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            var exams = await _context.ExamRequests
                .Include(e => e.ExamType)
                .Include(e => e.Address)
                .Where(e => e.PatientId == userId)
                .OrderByDescending(e => e.CreatedAt)
                .Select(e => new ExamRequestResponse
                {
                    Id = e.Id,
                    ExamType = e.ExamType!.Name,
                    Status = e.Status.ToString(),
                    Address = $"{e.Address!.Street}, {e.Address.Number}",
                    Result = e.Result,
                    CreatedAt = e.CreatedAt
                })
                .ToListAsync();

            return Ok(exams);
        }
    }
}
