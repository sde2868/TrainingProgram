using TraineeManagement.Models;

namespace TraineeManagement.Interfaces;
public interface IMessagePublisher
{
    Task PublishSubmissionProcessingAsync(
        SubmissionProcessingRequested message,
        CancellationToken cancellationToken = default);
}