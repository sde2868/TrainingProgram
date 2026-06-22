using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TraineeManagement.Data;
using TraineeManagement.Exceptions;
using TraineeManagement.Models;
using System.Security.Claims;
using TraineeManagement.Interfaces;

namespace TraineeManagement.Services;

public class SubmissionFileServices : ISubmissionFileService
{
    private readonly AppDbContext _context;
    private readonly IFileStorageService _fileStorage;
    private readonly ILogger<SubmissionFileServices> _logger;
    private readonly FileUploadSettings _fileUploadSettings;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public SubmissionFileServices(
        AppDbContext context,
        IFileStorageService fileStorage,
        ILogger<SubmissionFileServices> logger,
        IOptions<FileUploadSettings> fileUploadOptions,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _fileStorage = fileStorage;
        _logger = logger;
        _fileUploadSettings = fileUploadOptions.Value;
        _httpContextAccessor = httpContextAccessor;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim =
            _httpContextAccessor.HttpContext?
                .User
                .FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            throw new UnauthorizedAccessException(
                "User id not found in token.");
        }

        return int.Parse(userIdClaim.Value);
    }

    public async Task<SubmissionFileDTO> UploadAsync(
        int submissionId,
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Configured MaxFileSizeBytes: {Size}", _fileUploadSettings.MaxFileSizeBytes);
        _logger.LogInformation("Upload request received. File size: {Size}", file?.Length);
        var submission =
            await _context.TaskSubmissions
                .FirstOrDefaultAsync(
                    x => x.Id == submissionId,
                    cancellationToken);

        if (submission == null)
        {
            throw new KeyNotFoundException(
                $"Submission with id {submissionId} not found.");
        }

        if (file == null)
        {
            throw new ArgumentException(
                "File is required.");
        }

        if (file.Length == 0)
        {
            throw new ArgumentException(
                "Empty files are not allowed.");
        }

        if (file.Length > _fileUploadSettings.MaxFileSizeBytes)
        {
            throw new FileTooLargeException(
                $"Maximum allowed file size is {_fileUploadSettings.MaxFileSizeBytes / (1024 * 1024)} MB.");
            // throw new ArgumentException("TEST");
        }

        string extension =
            Path.GetExtension(file.FileName)
                .ToLowerInvariant();

        if (!_fileUploadSettings.AllowedExtensions
                .Contains(extension))
        {
            throw new ArgumentException(
                $"Files with extension '{extension}' are not allowed.");
        }

        await using var stream =
            file.OpenReadStream();

        string storageName =
            await _fileStorage.SaveAsync(
                stream,
                extension,
                cancellationToken);

        var submissionFile = new SubmissionFile
        {
            SubmissionId = submissionId,
            OriginalFileName = file.FileName,
            StorageName = storageName,
            ContentType = file.ContentType,
            Size = file.Length,
            Checksum = string.Empty, // TODO: SHA256
            UploadedByUserId = GetCurrentUserId(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.SubmissionFiles.AddAsync(
            submissionFile,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation(
            "Submission file uploaded successfully. FileId: {FileId}, SubmissionId: {SubmissionId}",
            submissionFile.Id,
            submissionId);

        return ReturnDTO(submissionFile);
    }

    public async Task<SubmissionFileDTO?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var file =
            await _context.SubmissionFiles
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        return file == null
            ? null
            : ReturnDTO(file);
    }

    public async Task<(Stream Stream, SubmissionFile File)> DownloadAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var file =
            await _context.SubmissionFiles
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (file == null)
        {
            throw new FileNotFoundException(
                "File metadata not found.");
        }

        bool exists =
            await _fileStorage.ExistsAsync(
                file.StorageName,
                cancellationToken);

        if (!exists)
        {
            throw new FileNotFoundException(
                "Physical file not found.");
        }

        var stream =
            await _fileStorage.OpenReadAsync(
                file.StorageName,
                cancellationToken);

        return (stream, file);
    }

    public async Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var file =
            await _context.SubmissionFiles
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (file == null)
        {
            throw new FileNotFoundException(
                "File not found.");
        }

        await _fileStorage.DeleteAsync(
            file.StorageName,
            cancellationToken);

        _context.SubmissionFiles.Remove(file);

        await _context.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation(
            "Submission file deleted successfully. FileId: {FileId}",
            file.Id);
    }

    public SubmissionFileDTO ReturnDTO(
        SubmissionFile file)
    {
        return new SubmissionFileDTO
        {
            Id = file.Id,
            SubmissionId = file.SubmissionId,
            OriginalFileName = file.OriginalFileName,
            ContentType = file.ContentType,
            Size = file.Size,
            Checksum = file.Checksum,
            UploadedByUserId = file.UploadedByUserId,
            CreatedAt = file.CreatedAt,
            UpdatedAt = file.UpdatedAt
        };
    }
}