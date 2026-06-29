public class SubmissionQueuedResponseDTO
{
    public int TaskSubmissionId { get; set; }

    public int SubmissionFileId { get; set; }

    public Guid MessageId { get; set; }

    public string Status { get; set; } = "Queued";
}