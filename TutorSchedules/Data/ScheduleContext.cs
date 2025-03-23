using Microsoft.EntityFrameworkCore;
using TutorSchedules.Models;

namespace TutorSchedules.Data;

public class ScheduleContext : DbContext
{
	public DbSet<Tutor> Tutors { get; set; }
	public DbSet<TimeBlock> ScheduleBlocks { get; set; }
	public DbSet<Subject> Subjects { get; set; }
	
	public ScheduleContext(DbContextOptions<ScheduleContext> options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
	}
}