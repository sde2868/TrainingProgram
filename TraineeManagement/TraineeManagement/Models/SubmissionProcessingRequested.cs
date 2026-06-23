namespace TraineeManagement.Models;
public class SubmissionProcessingRequested
{
    public Guid MessageId { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
    public int TaskSubmissionId { get; set; }
    public int SubmissionFileId { get; set; }
    public DateTime RequestedAt { get; set; }
    public string ContractVersion { get; set; } = "1.0";
}