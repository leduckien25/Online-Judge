namespace OnlineJudge.Sandbox.Services
{
    public interface ISubmissionQueue
    {
        Task EnqueueAsync(string submissionId);
        Task<string> DequeueAsync(CancellationToken cancellationToken);
    }
}
