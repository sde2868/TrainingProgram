using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TraineeManagement.Models;

public enum TaskSubmissionStatus
{
    Submitted,
    Resubmitted
}

public class TaskSubmission
{
    [Key]
    [Required(ErrorMessage = "Id is required!")]
    public int Id { get; set; }
    [Required(ErrorMessage = "TaskAssignmentId is required!")]
    public int TaskAssignmentId { get; set; }
    [Required(ErrorMessage = "SubmissionUrl is requried!")]
    public string SubmissionUrl { get; set; }
    public string Notes { get; set; }
    public DateTime SubmittedDate { get; set; }
    public TaskSubmissionStatus Status { get; set; }

    // Navigation
    public TaskAssignment TaskAssignment { get; set; }
    public List<Review> Reviews { get; set; }
    [JsonIgnore]
    public List<SubmissionFile> Files { get; set; } = new();
}