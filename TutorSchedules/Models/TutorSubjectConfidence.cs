using System.ComponentModel.DataAnnotations;

namespace TutorSchedules.Models;

public class TutorSubjectConfidence
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int TutorId { get; set; }
    public Tutor? Tutor { get; set; }
    [Required]
    public string SubjectId { get; set; }
    public Subject? Subject { get; set; }
    
    public int? ConfidenceLevel { get; set; }
}