using Microsoft.AspNetCore.Mvc;
using OnlineJudge.Core.Data;
using OnlineJudge.Api.Dtos;
using OnlineJudge.Core.Models;
using OnlineJudge.Api.Responses;
using Microsoft.EntityFrameworkCore;

namespace OnlineJudge.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/problems")]
    public class AdminProblemsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminProblemsController(AppDbContext context) => _context = context;

        [HttpPost]
        public async Task<IActionResult> CreateProblem([FromBody] ProblemCreateDto createProblemDto)
        {
            var problem = new Problem
            {
                Title = createProblemDto.Title,
                Description = createProblemDto.Description,
                TimeLimitMs = createProblemDto.TimeLimitMs
            };

            await _context.Problems.AddAsync(problem);

            var rowsAffected = await _context.SaveChangesAsync();

            if (rowsAffected > 0)
            {
                return Ok(ApiResponse<Problem>.Ok(problem));
            }

            return BadRequest(ApiResponse<Problem>.Fail("Failed to create problem."));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditAsync(string id, [FromBody] ProblemUpdateDto dto)
        {
            var problem = await _context.Problems.FirstOrDefaultAsync(p => p.Id == id);
            if (problem == null)
            {
                return NotFound(ApiResponse<object>.Fail("Problem not found."));
            }

            problem.Title = dto.Title;
            problem.Description = dto.Description;
            problem.TimeLimitMs = dto.TimeLimitMs;
            problem.Difficulty = dto.Difficulty;
            problem.Example = dto.Example;

            await _context.SaveChangesAsync();
            return Ok(ApiResponse<object?>.Ok(null, "Problem updated successfully."));
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
                    return Ok(ApiResponse<object?>.Ok(null, "Problem deleted successfully."));
                }
                return BadRequest(ApiResponse<object>.Fail("Failed to delete problem."));
            }
            return NotFound(ApiResponse<object>.Fail("Problem not found."));
        }

        [HttpPost("{problemId}/testcases")]
        public async Task<IActionResult> AddTestCases(string problemId, [FromBody] List<TestCaseCreateDto> testCases)
        {
            var existingProblem = await _context.Problems.FindAsync(problemId);
            if (existingProblem != null)
            {
                int rowsAffected;

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
                    return Ok(ApiResponse<object?>.Ok(null, "Test cases added successfully."));
                }
                return BadRequest(ApiResponse<object>.Fail("Failed to add test cases."));
            }
            return NotFound(ApiResponse<object>.Fail("Problem not found."));
        }

        [HttpGet("{id}/testcases")]
        public async Task<IActionResult> GetTestCasesByProblemId(string id)
        {
            var problemExists = await _context.Problems.AnyAsync(p => p.Id == id);
            if (!problemExists)
            {
                return NotFound(ApiResponse<object?>.Fail("Problem not found."));
            }

            var testCases = await _context.TestCases
                .Where(tc => tc.ProblemId == id)
                .OrderBy(tc => tc.Id)
                .Select(tc => new TestCaseResponseDto
                {
                    Id = tc.Id,
                    InputData = tc.InputData,
                    ExpectedOutput = tc.ExpectedOutput
                })
                .ToListAsync();

            return Ok(ApiResponse<List<TestCaseResponseDto>>.Ok(testCases, "Test cases retrieved successfully."));
        }

        [HttpPost("{id}/testcases/sync")]
        public async Task<IActionResult> SyncTestCasesAsync(string id, [FromBody] TestCaseUpdateDto[] dtos)
        {
            var problemExists = await _context.Problems.AnyAsync(p => p.Id == id);

            if (!problemExists)
            {
                return NotFound(ApiResponse<object>.Fail("Problem not found."));
            }

            var validDtos = dtos
                .Where(d => !string.IsNullOrEmpty(d.Id) ||
                            !string.IsNullOrWhiteSpace(d.InputData) ||
                            !string.IsNullOrWhiteSpace(d.ExpectedOutput))
                .ToList();

            var updateDtos = validDtos.Where(d => !string.IsNullOrEmpty(d.Id));

            var existingTestCases = await _context.TestCases.Where(tc => tc.ProblemId == id).ToListAsync(); ;

            var testCasesToDelete = existingTestCases.Where(tc => !updateDtos.Any(d => d.Id == tc.Id)).ToList();

            foreach (var d in updateDtos)
            {
                var existingTestCase = existingTestCases.FirstOrDefault(t => t.Id == d.Id);
                if (existingTestCase != null)
                {
                    existingTestCase.InputData = d.InputData;
                    existingTestCase.ExpectedOutput = d.ExpectedOutput;
                }
            }

            _context.TestCases.RemoveRange(testCasesToDelete);

            var newTestCases = validDtos.Where(d => string.IsNullOrEmpty(d.Id)).Select(d => new TestCase
            {
                InputData = d.InputData,
                ExpectedOutput = d.ExpectedOutput,
                ProblemId = id
            });

            await _context.TestCases.AddRangeAsync(newTestCases);

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object?>.Ok(null, "Test cases synced successfully."));
        }
    }
}
