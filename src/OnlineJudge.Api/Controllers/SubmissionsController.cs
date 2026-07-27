using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineJudge.Core.Data;
using OnlineJudge.Api.Dtos;
using OnlineJudge.Core.Models;
using OnlineJudge.Api.Responses;
using OnlineJudge.Sandbox.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

namespace OnlineJudge.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubmissionsController : ControllerBase
    {
        private readonly ISubmissionQueue _queue;
        private readonly AppDbContext _context;

        public SubmissionsController(ISubmissionQueue queue, AppDbContext context)
        {
            _queue = queue;
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubmissions(string id)
        {
            var submission = await _context.Submissions.FirstOrDefaultAsync(s => s.Id == id);

            if (submission == null) {
                return NotFound(ApiResponse<object>.Fail("Submission not found"));
            }

            return Ok(ApiResponse<ResponseSubmissionDto>.Ok(new ResponseSubmissionDto
            {
                Id = submission.Id,
                ProblemId = submission.ProblemId,
                SourceCode = submission.SourceCode,
                SubmittedAt = submission.SubmittedAt,
                Status = submission.Status,
            }));
        }

        [HttpPost]
        public async Task<IActionResult> SubmitCode([FromBody] SubmissionCreateDto dto)
        {
            var submission = new Submission
            {
                ProblemId = dto.ProblemId,
                SourceCode = dto.SourceCode
            };

            try
            {
                await _context.Submissions.AddAsync(submission);
                int rowsAffected =   await _context.SaveChangesAsync();

                if (rowsAffected < 1)
                {
                    throw new Exception("Failed to save submission to the database.");
                }

                // 3. Map the entity to your Response DTO
                var responseDto = new ResponseSubmissionDto
                {
                    Id = submission.Id,
                    ProblemId = submission.ProblemId,
                    SourceCode = dto.SourceCode,
                    SubmittedAt = submission.SubmittedAt
                };

                // 4. Push the ID to the background worker queue
                await _queue.EnqueueAsync(submission.Id);

                // 5. Return the clean DTO inside your API response wrapper
                return Ok(ApiResponse<ResponseSubmissionDto>.Ok(responseDto));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<Submission>.Fail($"Failed to submit code. Error: {ex.Message}"));
            }
        }


    }
}
