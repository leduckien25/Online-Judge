using Microsoft.AspNetCore.Mvc;
using OnlineJudge.Core.Data;
using OnlineJudge.Core.Models;
using OnlineJudge.Api.Responses;
using Microsoft.EntityFrameworkCore;
using OnlineJudge.Api.Dtos;

namespace OnlineJudge.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProblemsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProblemsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProblemAsync(string id)
        {
            var existingProblem = await _context.Problems.FindAsync(id);

            if (existingProblem != null) {
                var problemDto = new ProblemResponseDto
                {
                    Id = existingProblem.Id,
                    Title = existingProblem.Title,
                    Difficulty = existingProblem.Difficulty,
                    Description = existingProblem.Description,
                    TimeLimitMs = existingProblem.TimeLimitMs
                };

                return Ok(ApiResponse<ProblemResponseDto>.Ok(problemDto));
            }

            return NotFound(ApiResponse<object>.Fail("Problem not found"));
        }

        [HttpGet]
        public async Task<IActionResult> GetProblems()
        {
            var problems = await _context.Problems.ToListAsync();

            if(problems.Count> 0)
            {
                var problemDtos = problems.Select(p => new ProblemResponseDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Difficulty = p.Difficulty,
                    TimeLimitMs = p.TimeLimitMs
                }).ToList();

                return Ok(ApiResponse<IEnumerable<ProblemResponseDto>>.Ok(problemDtos));
            }

            return NotFound(ApiResponse<object>.Fail("No problems found"));
        }


    }

}