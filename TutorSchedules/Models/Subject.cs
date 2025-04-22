using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TutorSchedules.Models;

public class Subject
{
    [Key]
    [RegularExpression(@"^[A-Za-z]{4}-\d{4}$", ErrorMessage = "Course number must be in the format: XXXX-####")]
    public string SubjectId { get; set; } = "";
    [Required]
    public int Order { get; set; }

    [Required]
    [StringLength(30, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 30 characters long.")]
    public string Name { get; set; } = "";

    [JsonIgnore] public ICollection<TutorSubjectConfidence> TutorConfidences { get; set; } = new List<TutorSubjectConfidence>();
}