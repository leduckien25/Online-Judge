using Microsoft.AspNetCore.Mvc;
using OnlineJudge.Core.Data;
using OnlineJudge.Core.Models;
using OnlineJudge.Api.Responses;

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
                return Ok(ApiResponse<Problem>.Ok(existingProblem));
            }

            return NotFound(ApiResponse<object>.Fail("Problem not found"));
        }


    }

}