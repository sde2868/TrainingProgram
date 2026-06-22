namespace TraineeManagement.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveAsync(
        Stream fileStream,
        string fileExtension,
        CancellationToken cancellationToken = default);

    Task<Stream> OpenReadAsync(
        string storageName,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        string storageName,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string storageName,
        CancellationToken cancellationToken = default);
}