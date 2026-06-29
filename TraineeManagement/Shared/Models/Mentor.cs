using System.ComponentModel.DataAnnotations;

namespace TraineeManagement.Models;

public enum MentorStatus
{
    Active,
    Inactive
}

public class Mentor
{
    [Key]
    [Required(ErrorMessage = "Id is required!")]
    public int Id { get; set; }
    [Required(ErrorMessage = "FirstName is required!")]
    [StringLength(50, MinimumLength = 3)]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "FirstName should contain only letters!")]
    public string FirstName { get; set; }
    [Required(ErrorMessage = "LastName is required!")]
    [StringLength(50, MinimumLength = 3)]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "LastName should contain only letters!")]
    public string LastName { get; set; }
    [Required(ErrorMessage = "Email is required!")]
    [StringLength(50, MinimumLength = 3)]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string[] Expertise { get; set; } = Array.Empty<string>();
    [Required]
    public MentorStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}