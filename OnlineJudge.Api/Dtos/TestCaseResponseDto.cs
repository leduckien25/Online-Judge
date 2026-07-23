using System.ComponentModel.DataAnnotations;

namespace OnlineJudge.Api.Dtos
{
    public class TestCaseResponseDto
    {
        [Required]
        public string Id { get; set; } = null!;
        [Required]
        public string InputData { get; set; } = null!;

        [Required]
        public string ExpectedOutput { get; set; } = null!;
    }
}
