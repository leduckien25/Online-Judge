using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineJudge.Core.Data;
using OnlineJudge.Api.Dtos;
using OnlineJudge.Core.Models;
using OnlineJudge.Api.Responses;
using OnlineJudge.Sandbox.Services;

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

        [HttpPost]
        public async Task<IActionResult> SubmitCode([FromBody] SubmissionDto dto)
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

                await _queue.EnqueueAsync(submission.Id);

                return Ok(ApiResponse<Submission>.Ok(submission));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<Submission>.Fail($"Failed to submit code. Error: {ex.Message}"));
            }
        }


    }
}
