using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TraineeManagement.Models;

public enum UserRole
{
    Admin,
    Mentor,
    Trainee
}

[Index(nameof(UserName), IsUnique = true)]
[Index(nameof(Email), IsUnique = true)]
public class User
{
    [Key]
    [Required(ErrorMessage = "Id is required!")]
    public int Id { get; set; }
    [Required(ErrorMessage = "UserName is required!")]
    [StringLength(50, MinimumLength = 3)]
    [RegularExpression(@"^[a-zA-Z_]+$", ErrorMessage = "UserName should contain only letters and underscores!")]
    public string UserName { get; set; }
    [Required(ErrorMessage = "Email is required!")]
    [StringLength(50, MinimumLength = 3)]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}