using System.ComponentModel.DataAnnotations;


namespace OnlineJudge.Core.Models
{
    public class Problem
    {
        [Key]
        [MaxLength(6)]
        public string Id { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        public int TimeLimitMs { get; set; } = 2000;

        public ICollection<TestCase> TestCases { get; set; } = null!;
    }
}
