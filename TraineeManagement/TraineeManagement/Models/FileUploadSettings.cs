namespace TraineeManagement.Models;

public class FileUploadSettings
{
    public long MaxFileSizeBytes { get; set; }

    public List<string> AllowedExtensions { get; set; } = [];
}