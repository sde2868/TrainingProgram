using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TraineeManagement.Models;

public class SubmissionFile
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int SubmissionId { get; set; }

    [Required]
    [MaxLength(255)]
    public string OriginalFileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string StorageName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ContentType { get; set; } = string.Empty;

    [Required]
    public long Size { get; set; }

    [Required]
    [MaxLength(128)]
    public string Checksum { get; set; } = string.Empty;

    public int? UploadedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    // Navigation Property
    [JsonIgnore]
    public TaskSubmission Submission { get; set; } = null!;
}