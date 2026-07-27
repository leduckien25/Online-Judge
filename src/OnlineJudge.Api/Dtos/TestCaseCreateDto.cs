using System.ComponentModel.DataAnnotations;

namespace OnlineJudge.Api.Dtos
{
    public class TestCaseCreateDto
    {
        public string? InputData { get; set; } 

        public string? ExpectedOutput { get; set; } 
    }
}
