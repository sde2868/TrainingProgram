using System.ComponentModel.DataAnnotations;

namespace TraineeManagement.Models;

public class TaskSubmissionDTO
{
    [Required(ErrorMessage = "TaskAssignmentId is required!")]
    public int TaskAssignmentId { get; set; }
    [Required(ErrorMessage = "SubmissionUrl is requried!")]
    public string SubmissionUrl { get; set; }
    public string Notes { get; set; }
    public DateTime SubmittedDate { get; set; }
    public TaskSubmissionStatus Status { get; set; }
}