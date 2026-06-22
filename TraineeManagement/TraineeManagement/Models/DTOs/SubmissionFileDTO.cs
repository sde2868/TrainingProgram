using System.ComponentModel.DataAnnotations;

namespace TraineeManagement.Models;

public class SubmissionFileDTO
{
    public int Id { get; set; }

    public int SubmissionId { get; set; }

    public string OriginalFileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long Size { get; set; }

    public string Checksum { get; set; } = string.Empty;

    public int? UploadedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
