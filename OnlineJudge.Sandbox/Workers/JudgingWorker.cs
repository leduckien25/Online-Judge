using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OnlineJudge.Core.Data;
using OnlineJudge.Sandbox.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace OnlineJudge.Sandbox.Workers
{
    public class JudgingWorker : BackgroundService
    {
        private readonly ISubmissionQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ISandboxService _sandboxService;

        public JudgingWorker(ISubmissionQueue queue, IServiceScopeFactory scopeFactory, ISandboxService sandboxService)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
            _sandboxService = sandboxService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var submissionId = await _queue.DequeueAsync(stoppingToken);

                if (submissionId == null)
                    continue;

                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var submission = await dbContext.Submissions.FindAsync(submissionId, stoppingToken);

                    if (submission == null)
                    {
                        Console.WriteLine($"Submission {submissionId} not found in database. Skipping.");
                        continue;
                    }

                    var problem = await dbContext.Problems.Include(p => p.TestCases).FirstOrDefaultAsync(p => p.Id == submission.ProblemId, stoppingToken);

                    if (problem == null)
                    {
                        Console.WriteLine($"Problem {submission.ProblemId} not found in database. Skipping.");
                        submission.Status = "RE";
                        await dbContext.SaveChangesAsync(stoppingToken);
                        continue;
                    }

                    submission.Status = "AC";

                    foreach (var testcase in problem.TestCases)
                    {
                        var (time, output, error) = await _sandboxService.RunPythonCodeAsync(submission.SourceCode, testcase.InputData);

                        if (!string.IsNullOrEmpty(error))
                        {
                            submission.Status = "RE";
                            break;
                        }

                        if (time > problem.TimeLimitMs)
                        {
                            submission.Status = "TLE";
                            break;
                        }

                        string cleanOutput = (output ?? "").Trim();
                        string cleanExpected = (testcase.ExpectedOutput ?? "").Trim();

                        if (cleanOutput != cleanExpected)
                        {
                            submission.Status = "WA";
                            break;
                        }
                    }

                    Console.WriteLine($"Submission {submissionId} judged. Status: {submission.Status}");

                    await dbContext.SaveChangesAsync(stoppingToken);
                }
            }
        }
    }
}
