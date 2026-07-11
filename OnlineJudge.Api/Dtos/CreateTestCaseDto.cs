using System.ComponentModel.DataAnnotations;

namespace OnlineJudge.Api.Dtos
{
    public class CreateTestCaseDto
    {
        [Required(ErrorMessage = "Input data is required")]
        public string InputData { get; set; } = null!;

        [Required(ErrorMessage = "Expected output is required")]
        public string ExpectedOutput { get; set; } = null!;
    }
}
