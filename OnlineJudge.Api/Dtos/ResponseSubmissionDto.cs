namespace OnlineJudge.Api.Dtos
{
    public class ResponseSubmissionDto
    {
        public string Id { get; set; } = null!;
        public string ProblemId { get; set; } = null!;
        public string SourceCode { get; set; } = null!;
        public string Status { get; set; } = "Pending";
        public DateTime SubmittedAt { get; set; }
    }
}
