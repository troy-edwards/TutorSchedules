using Microsoft.EntityFrameworkCore;
using TutorSchedules.Models;

namespace TutorSchedules.Data;

public class ScheduleContext : DbContext
{
	public DbSet<Tutor> Tutors { get; set; }
	public DbSet<TimeBlock> ScheduleBlocks { get; set; }
	public DbSet<Subject> Subjects { get; set; }
	public DbSet<TutorSubjectConfidence> Confidences { get; set; }
	
	public ScheduleContext(DbContextOptions<ScheduleContext> options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<TutorSubjectConfidence>()
			.HasOne(tsc => tsc.Tutor)
			.WithMany(t => t.SubjectConfidences)
			.HasForeignKey(tsc => tsc.TutorId);

		modelBuilder.Entity<TutorSubjectConfidence>()
			.HasOne(tsc => tsc.Subject)
			.WithMany(s => s.TutorConfidences)
			.HasForeignKey(tsc => tsc.SubjectId);

		modelBuilder.Entity<TutorSubjectConfidence>()
			.Property(tsc => tsc.ConfidenceLevel)
			.HasDefaultValue(null);
	}
}