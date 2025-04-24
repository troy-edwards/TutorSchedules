using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TutorSchedules.Models;

public class TutorSubjectConfidence
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int TutorId { get; set; }
    [JsonIgnore]
    public Tutor? Tutor { get; set; }
    [Required] public string SubjectId { get; set; } = "";
    [JsonIgnore]
    public Subject? Subject { get; set; }
    
    public int? ConfidenceLevel { get; set; }
}