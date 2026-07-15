using OnlineJudge.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace OnlineJudge.Api.Dtos
{
    public class ResponseProblemDto
    {
        public string Id { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public int TimeLimitMs { get; set; } = 2000;

        public DifficultyLevel Difficulty { get; set; }

        public string? Example { get; set; }
    }
}
