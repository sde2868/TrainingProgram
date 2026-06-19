using TraineeManagement.Models;

namespace TraineeManagement.Services;

public interface ISubmissionFileService
{
    Task<SubmissionFileDTO> UploadAsync(
        int submissionId,
        IFormFile file,
        CancellationToken cancellationToken = default);

    Task<SubmissionFileDTO?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<(Stream Stream, SubmissionFile File)> DownloadAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);

    SubmissionFileDTO ReturnDTO(
        SubmissionFile file);
}