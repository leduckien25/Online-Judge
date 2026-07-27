using Microsoft.EntityFrameworkCore;
using OnlineJudge.Core.Data;
using Microsoft.AspNetCore.SignalR;

namespace OnlineJudge.Core.Hub
{
    public class SubmissionHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly AppDbContext _context;

        public SubmissionHub(AppDbContext context)
        {
            _context = context;
        }

        public async Task WatchSubmissionAsync(string submissionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, submissionId);

            var currentStatus = await _context.Submissions
                .AsNoTracking()
                .Where(s => s.Id == submissionId)
                .Select(s => s.Status)
                .FirstOrDefaultAsync();

            if (currentStatus != "Pending")
            { 
                await Clients.Caller.SendAsync("OnStatusUpdate", currentStatus);
            }
        }

        public async Task UnwatchSubmissionAsync(string submissionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, submissionId);
        }
    }
}
