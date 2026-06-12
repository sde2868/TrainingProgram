using System.ComponentModel.DataAnnotations;

namespace TraineeManagement.Models;

public enum TaskAssignmentStatus
{
    Assigned,
    InProgress,
    Submitted,
    Reviewed,
    Completed
}

public class TaskAssignment
{
    [Key]
    [Required(ErrorMessage = "Id is required!")]
    public int Id { get; set; }
    [Required(ErrorMessage = "TraineeId is required!")]
    public int TraineeId { get; set; }
    [Required(ErrorMessage = "MentorId is required!")]
    public int MentorId { get; set; }
    [Required(ErrorMessage = "LearningTaskId is required!")]
    public int LearningTaskId { get; set; }
    [Required(ErrorMessage = "Status is required!")]
    public TaskAssignmentStatus Status { get; set; }
    [Required(ErrorMessage = "AssignedDate is required!")]
    public DateTime AssignedDate { get; set; }
    [Required(ErrorMessage = "DueDate is required!")]
    public DateTime DueDate { get; set; }
    public string Remarks;

    // Navigation properties
    public Trainee Trainee { get; set; }
    public Mentor Mentor { get; set; }
    public LearningTask LearningTask { get; set; }
    public List<TaskSubmission> Submissions { get; set; }
}