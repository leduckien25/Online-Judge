using Microsoft.EntityFrameworkCore;
using OnlineJudge.Core.Models;

namespace OnlineJudge.Core.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Problem> Problems { get; set; } = null!;
        public DbSet<TestCase> TestCases { get; set; } = null!;
        public DbSet<Submission> Submissions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasSequence<int>("SubmissionSequence").StartsAt(1).IncrementsBy(1);
            modelBuilder.HasSequence<int>("ProblemSequence").StartsAt(1).IncrementsBy(1);
            modelBuilder.HasSequence<int>("TestCaseSequence").StartsAt(1).IncrementsBy(1);
            
            modelBuilder.Entity<Submission>()
                .Property(s => s.Id)
                .HasDefaultValueSql("'SUB' || lpad(nextval('\"SubmissionSequence\"')::text, 3, '0')");

            modelBuilder.Entity<Problem>()
                .Property(s=>s.Id)
                .HasDefaultValueSql("'PRO' || lpad(nextval('\"ProblemSequence\"')::text, 3, '0')");

            modelBuilder.Entity<Problem>()
            .Property(p => p.Difficulty)
            .HasConversion<string>() 
            .HasMaxLength(20);

            modelBuilder.Entity<TestCase>()
                .Property(s=>s.Id)
                .HasDefaultValueSql("'TC' || lpad(nextval('\"TestCaseSequence\"')::text, 3, '0')");

            modelBuilder.Entity<TestCase>()
                .HasOne(tc => tc.Problem)
                .WithMany(p => p.TestCases)
                .HasForeignKey(tc => tc.ProblemId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}