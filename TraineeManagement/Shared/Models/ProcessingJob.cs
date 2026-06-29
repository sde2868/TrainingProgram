namespace TraineeManagement.Models;
public enum ProcessingJobStatus
{
    Queued = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4
}

public class ProcessingJob
{
    public int Id { get; set; }
    public Guid MessageId { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
    public int SubmissionId { get; set; }
    public int SubmissionFileId { get; set; }
    public ProcessingJobStatus Status { get; set; }
    public int Attempts { get; set; }
    public string? ErrorSummary { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation Properties
    public TaskSubmission Submission { get; set; } = null!;
    public SubmissionFile SubmissionFile { get; set; } = null!;
}
