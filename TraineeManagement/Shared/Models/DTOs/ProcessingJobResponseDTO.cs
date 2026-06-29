namespace TraineeManagement.Models;
public class ProcessingJobResponseDTO
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public int Attempts { get; set; }
    public string? ErrorSummary { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
}
