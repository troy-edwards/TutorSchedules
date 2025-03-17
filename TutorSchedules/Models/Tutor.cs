namespace TutorSchedules.Models;

public class Tutor
{
	public int ID { get; set; }
	public string? DisplayName { get; set; }
	public TutorLevel Level { get; set; }
}