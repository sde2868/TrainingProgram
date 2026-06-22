using System.ComponentModel.DataAnnotations;

namespace TraineeManagement.Models;

public class LearningTaskDTO
{
    [Required(ErrorMessage = "Title is required!")]
    [StringLength(50, MinimumLength = 3)]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Title should contain only letters!")]
    public string Title { get; set; }
    [Required(ErrorMessage = "Description is required!")]
    [StringLength(5000, MinimumLength = 3)]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Description should contain only letters!")]
    public string Description { get; set; }
    [Required]
    public string[] ExpectedTechStack { get; set; } = Array.Empty<string>();
    [Required]
    public LearningTaskStatus Status { get; set; }
    [Required]
    public DateTime DueDate { get; set; }
}