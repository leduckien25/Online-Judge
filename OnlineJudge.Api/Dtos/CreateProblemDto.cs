using OnlineJudge.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace OnlineJudge.Api.Dtos
{
    public class CreateProblemDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Time limit is required")]
        public int TimeLimitMs { get; set; } = 2000;

        [Required(ErrorMessage = "Test cases are required")]
        public ICollection<CreateTestCaseDto> TestCases { get; set; } = null!;

    }
}
