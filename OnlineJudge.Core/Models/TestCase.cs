using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OnlineJudge.Core.Models
{
    public class TestCase
    {
        [Key]
        [MaxLength(6)]
        public string Id { get; set; } = null!;
        [Required]
        [MaxLength(6)]
        public string ProblemId { get; set; } = null!;

        [Required]
        public string InputData { get; set; } = null!;

        [Required]
        public string ExpectedOutput { get; set; } = null!;

        [ForeignKey(nameof(ProblemId))]
        [JsonIgnore] 
        public Problem Problem { get; set; } = null!;
    }
}
