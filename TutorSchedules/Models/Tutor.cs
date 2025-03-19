using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TutorSchedules.Models;

public class Tutor
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int Id { get; set; }
	[Required]
	public string DisplayName { get; set; }
	//public TutorLevel Level { get; set; }
}