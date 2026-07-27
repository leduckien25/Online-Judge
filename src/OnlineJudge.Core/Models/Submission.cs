using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineJudge.Core.Models
{
    public class Submission
    {
        [Key]
        [MaxLength(6)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; } = null!;

        [Required]
        [MaxLength(6)]
        public string ProblemId { get; set; } = null!;
        //public int UserId { get; set; } = 1; 

        [Required]
        public string SourceCode { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "Pending";

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [ForeignKey(nameof(ProblemId))]
        public Problem Problem { get; set; } = null!;
    }
}
