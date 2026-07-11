using System.Threading.Channels;

namespace OnlineJudge.Sandbox.Services
{
    public class SubmissionQueue : ISubmissionQueue
    {
        private readonly Channel<string> _queue = Channel.CreateUnbounded<string>();

        public async Task EnqueueAsync(string submissionId) => await _queue.Writer.WriteAsync(submissionId);

        public async Task<string> DequeueAsync(CancellationToken cancellationToken) => await _queue.Reader.ReadAsync(cancellationToken);
    }
}
