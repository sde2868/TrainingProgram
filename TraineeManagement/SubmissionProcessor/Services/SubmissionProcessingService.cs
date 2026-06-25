using TraineeManagement.Models;
using TraineeManagement.Interfaces;
using TraineeManagement.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using SubmissionProcessor.Configuration;
using Microsoft.Extensions.Options;
using SubmissionProcessor.Exceptions;
using SubmissionProcessor.Services;
using SubmissionProcessor.Models;

namespace SubmissionProcessor.Services;

public class SubmissionProcessingService : ISubmissionProcessingService
{
    private readonly AppDbContext _context;
    private readonly IFileStorageService _storage;
    private readonly ILogger<SubmissionProcessingService> _logger;
    private readonly ProcessingOptions _processingOptions;
    private readonly ITrainingDirectoryClient _trainingDirectoryClient;
    private async Task<TraineeProcessingProfileResponse?> GetTraineeProfileAsync(SubmissionProcessingRequested message, CancellationToken cancellationToken)
    {
            var submission = await _context.TaskSubmissions
                .Include(x => x.TaskAssignment)
                .FirstOrDefaultAsync(x => x.Id == message.TaskSubmissionId,
                cancellationToken);

            if (submission == null)
            {
                throw new InvalidOperationException($"Submission {message.TaskSubmissionId} not found.");
            }

            try
            {
                var profile = await _trainingDirectoryClient
                    .GetProcessingProfileAsync(
                        submission.TaskAssignment.TraineeId,
                        message.CorrelationId,
                        cancellationToken);

                _logger.LogInformation("Retrieved trainee profile for TraineeId {TraineeId}", profile?.TraineeId);
                return profile;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "TrainingDirectory unavailable for SubmissionId {SubmissionId}", message.TaskSubmissionId);
                return null;
            }
    }
    public SubmissionProcessingService(
        AppDbContext context,
        IFileStorageService storage,
        ILogger<SubmissionProcessingService> logger,
        IOptions<ProcessingOptions> processingOptions,
        ITrainingDirectoryClient trainingDirectoryClient)
    {
        _context = context;
        _storage = storage;
        _logger = logger;
        _processingOptions = processingOptions.Value;
        _trainingDirectoryClient = trainingDirectoryClient;
    }

    public async Task ProcessAsync(
        SubmissionProcessingRequested message,
        CancellationToken cancellationToken)
    {
        var job = await _context.ProcessingJobs.FirstOrDefaultAsync(x => x.MessageId == message.MessageId, cancellationToken);
        if (job == null)
        {
            _logger.LogWarning("Message received without matching ProcessingJob. MessageId: {MessageId}", message.MessageId);
            return;
        }
        // if (job.Status == ProcessingJobStatus.Completed)
        // {
        //     _logger.LogWarning("Message {MessageId} already completed.", message.MessageId);
        //     return;
        // }

        // var alreadyCompletedForFile = await _context.ProcessingJobs.AnyAsync(
        //             x => x.SubmissionFileId == message.SubmissionFileId &&
        //                  x.Status == ProcessingJobStatus.Completed &&
        //                  x.MessageId != message.MessageId,
        //                  cancellationToken);
        // if (alreadyCompletedForFile)
        // {
        //     _logger.LogWarning("File already processed. FileId {FileId}", message.SubmissionFileId);
        //     return;
        // }

        try
        {
            if (job.StartedAt == null)
            {
                job.StartedAt = DateTime.UtcNow;
            }
            job.Attempts++;
            job.Status = ProcessingJobStatus.Processing;
            job.ErrorSummary = null;

            await _context.SaveChangesAsync(cancellationToken);

            var file = await _context.SubmissionFiles.FirstOrDefaultAsync(x => x.Id == message.SubmissionFileId, cancellationToken);
            if (file.OriginalFileName.Contains("fail"))
            {
                throw new InvalidOperationException(
                    "Intentional test failure");
            }
            if (file == null)
            {
                throw new FileNotFoundException("Submission file not found.");
            }

            await using var stream = await _storage.OpenReadAsync(file.StorageName, cancellationToken);

            using var sha = SHA256.Create();
            var hash = await sha.ComputeHashAsync(stream, cancellationToken);
            var checksum = Convert.ToHexString(hash);

            file.Checksum = checksum;
            file.UpdatedAt = DateTime.UtcNow;

            job.Status = ProcessingJobStatus.Completed;
            job.CompletedAt = DateTime.UtcNow;
            job.ErrorSummary = null;

            var profile = await GetTraineeProfileAsync(message,
                cancellationToken);
            if (profile != null)
            {
                _logger.LogInformation("Retrieved trainee profile for TraineeId {TraineeId}", profile.TraineeId);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        catch (Exception ex)
        {
            job.ErrorSummary = ex.Message;
            job.CompletedAt = DateTime.UtcNow;

            if (job.Attempts >= _processingOptions.MaxAttempts)
            {
                job.Status = ProcessingJobStatus.Failed;
                _logger.LogError("Max retries reached for MessageId {MessageId}",
                    message.MessageId);
                await _context.SaveChangesAsync(cancellationToken);
                throw; // permanent failure
            }
            job.Status = ProcessingJobStatus.Queued;
            _logger.LogWarning("Retry attempt {Attempt} of {MaxAttempts} for MessageId {MessageId}",
                job.Attempts,
                _processingOptions.MaxAttempts,
                message.MessageId);
            await _context.SaveChangesAsync(cancellationToken);
            throw new RetryableProcessingException(ex.Message);
        }
    }
}