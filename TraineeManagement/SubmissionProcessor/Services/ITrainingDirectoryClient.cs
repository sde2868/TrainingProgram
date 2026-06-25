using SubmissionProcessor.Models;

namespace SubmissionProcessor.Services;
public interface ITrainingDirectoryClient
{
    Task<TraineeProcessingProfileResponse?> GetProcessingProfileAsync(
        int traineeId,
        string correlationId,
        CancellationToken cancellationToken);
}