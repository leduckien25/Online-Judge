using Microsoft.AspNetCore.Mvc;
using OnlineJudge.Core.Data;
using OnlineJudge.Api.Dtos;
using OnlineJudge.Core.Models;
using OnlineJudge.Api.Responses;

namespace OnlineJudge.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/problems")]
    public class AdminProblemsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminProblemsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProblemDto createProblemDto)
        {
            var problem = new Problem
            {
                Title = createProblemDto.Title,
                Description = createProblemDto.Description,
                TimeLimitMs = createProblemDto.TimeLimitMs,
                TestCases = createProblemDto.TestCases.Select(tc => new TestCase
                {
                    InputData = tc.InputData,
                    ExpectedOutput = tc.ExpectedOutput
                }).ToList()
            };

            await _context.Problems.AddAsync(problem);

            var rowsAffected = await _context.SaveChangesAsync();

            if (rowsAffected > 0)
            {
                return Ok(ApiResponse<Problem>.Ok(problem));
            }

            return BadRequest(ApiResponse<Problem>.Fail("Failed to create problem."));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var existingProblem = await _context.Problems.FindAsync(id);

            if (existingProblem != null)
            {
                _context.Problems.Remove(existingProblem);
                var rowsAffected = await _context.SaveChangesAsync();

                if (rowsAffected > 0)
                {
                    return Ok(ApiResponse<object>.Ok(null, "Problem deleted successfully."));
                }
                return BadRequest(ApiResponse<object>.Fail("Failed to delete problem."));
            }
            return NotFound(ApiResponse<object>.Fail("Problem not found."));
        }

        [HttpPost("{problemId}/testcases")]
        public async Task<IActionResult> AddTestCases(string problemId,[FromBody] List<CreateTestCaseDto> testCases)
        {
            var existingProblem = await _context.Problems.FindAsync(problemId);
            if (existingProblem != null)
            {
                int rowsAffected = 0;

                try
                {
                    foreach (var testCaseDto in testCases)
                    {
                        var testCase = new TestCase
                        {
                            InputData = testCaseDto.InputData,
                            ExpectedOutput = testCaseDto.ExpectedOutput,
                            ProblemId = problemId
                        };
                        await _context.TestCases.AddAsync(testCase);
                    }
                    rowsAffected = await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return BadRequest(ApiResponse<object>.Fail($"Failed to add test cases. Error: {ex.Message}"));
                }

                if (rowsAffected > 0)
                {
                    return Ok(ApiResponse<object>.Ok(null, "Test cases added successfully."));
                }
                return BadRequest(ApiResponse<object>.Fail("Failed to add test cases."));
            }
            return NotFound(ApiResponse<object>.Fail("Problem not found."));
        }
    }
}
