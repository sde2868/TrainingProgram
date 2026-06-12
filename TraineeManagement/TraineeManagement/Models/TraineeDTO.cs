using System.ComponentModel.DataAnnotations;

namespace TraineeManagement.Models
{
    public class TraineeDTO
    {
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
        public string[] TechStack { get; set; } = Array.Empty<string>();
        [Required]
        public TraineeStatus Status { get; set; }
    }
}