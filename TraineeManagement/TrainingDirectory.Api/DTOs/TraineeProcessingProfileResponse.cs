namespace TrainingDirectory.Api.DTOs;
public class TraineeProcessingProfileResponse
{
    public int TraineeId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string[] TechStack { get; set; } = Array.Empty<string>();
    public int TotalAssignments { get; set; }
    public int CompletedAssignments { get; set; }
}