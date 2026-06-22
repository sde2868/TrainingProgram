using System.ComponentModel.DataAnnotations;

namespace TraineeManagement.Models;

public class ReviewDTO
{
    [Required(ErrorMessage = "SubmissionId is required!")]
    public int TaskSubmissionId { get; set; }
    [Required(ErrorMessage = "MentorId is required!")]
    public int MentorId { get; set; }
    [Required(ErrorMessage = "Feedback is required!")]
    public string Feedback { get; set; }
    public int Score { get; set; }
    [Required]
    public ReviewStatus Status { get; set; }
    [Required(ErrorMessage = "ReviewedDate is required!")]
    public DateTime ReviewedDate { get; set; }
}