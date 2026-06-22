using Microsoft.Extensions.Options;
using TraineeManagement.Models;
using TraineeManagement.Interfaces;

namespace TraineeManagement.Services;

public class LocalFileStorageServices : IFileStorageService
{
    private readonly string _rootPath;
    private readonly ILogger<LocalFileStorageServices> _logger;

    public LocalFileStorageServices(
        IOptions<StorageSettings> options,
        ILogger<LocalFileStorageServices> logger)
    {
        _rootPath = options.Value.RootPath;
        _logger = logger;

        Directory.CreateDirectory(_rootPath);
    }

    public async Task<string> SaveAsync(
        Stream fileStream,
        string fileExtension,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"SaveAsync: saving file with extension {fileExtension}.");
        var storageName =
            $"{Guid.NewGuid():N}{fileExtension}";

        var filePath =
            Path.Combine(_rootPath, storageName);

        await using var outputStream =
            new FileStream(
                filePath,
                FileMode.CreateNew,
                FileAccess.Write);

        await fileStream.CopyToAsync(
            outputStream,
            cancellationToken);

        _logger.LogInformation(
            "File stored with name {StorageName}",
            storageName);

        return storageName;
    }

    public Task<Stream> OpenReadAsync(
        string storageName,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"OpenReadAsync: opening {storageName}.");
        var filePath =
            Path.Combine(_rootPath, storageName);

        Stream stream =
            new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);

        return Task.FromResult(stream);
    }

    public Task<bool> ExistsAsync(
        string storageName,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"ExistsAsync: checking {storageName}.");
        var filePath =
            Path.Combine(_rootPath, storageName);

        return Task.FromResult(
            File.Exists(filePath));
    }

    public Task DeleteAsync(
        string storageName,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"DeleteAsync: deleting {storageName}.");
        var filePath =
            Path.Combine(_rootPath, storageName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);

            _logger.LogInformation(
                "Deleted file {StorageName}",
                storageName);
        }

        return Task.CompletedTask;
    }
}