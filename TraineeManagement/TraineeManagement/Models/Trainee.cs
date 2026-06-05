using Microsoft.EntityFrameworkCore;

namespace TraineeManagement.Models;

public class Trainee
{
    public int? id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string[] TechStack { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

}