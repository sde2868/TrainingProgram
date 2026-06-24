using TraineeManagement.Models;

namespace SubmissionProcessor.Services;
public interface ISubmissionProcessingService
{
    Task ProcessAsync(
        SubmissionProcessingRequested message,
        CancellationToken cancellationToken);
}