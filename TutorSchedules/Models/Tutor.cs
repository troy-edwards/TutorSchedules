using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TutorSchedules.Models;

public class Tutor
{
	[Range(0,9999999, ErrorMessage = "Id can be up to 7 digits long.")]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int Id { get; set; }

	[Required]
	[StringLength(30, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 30 characters long.")]
	public string DisplayName { get; set; } = "";

	public ICollection<TimeBlock>? ScheduledTimes { get; set; } = new List<TimeBlock>();

	public ICollection<TutorSubjectConfidence> SubjectConfidences { get; set; } =
		new List<TutorSubjectConfidence>();
	//public TutorLevel Level { get; set; }
}