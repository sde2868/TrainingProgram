using System.ComponentModel.DataAnnotations;

public enum UserRole
{
    Admin,
    Mentor,
    Trainee
}

namespace TraineeManagement.Models
{
    public class UserDTO
    {
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
    }
}