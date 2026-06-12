using System.ComponentModel.DataAnnotations;

namespace TraineeManagement.Models;

public class TaskAssignmentDTO
{
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
}