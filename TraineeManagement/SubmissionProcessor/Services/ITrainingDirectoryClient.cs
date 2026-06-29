using Shared.DTOs;

namespace SubmissionProcessor.Services;
public interface ITrainingDirectoryClient
{
    Task<TraineeProcessingProfileResponse?> GetProcessingProfileAsync(
        int traineeId,
        string correlationId,
        CancellationToken cancellationToken);
}