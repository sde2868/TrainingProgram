using TraineeManagement.Interfaces;
using TraineeManagement.Data;
using TraineeManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace TraineeManagement.Services;

public class ProcessingJobServices : IProcessingJobService
{
    private readonly AppDbContext _context;
    public ProcessingJobServices(AppDbContext context)
    {
        _context = context;
    }
    public async Task<ProcessingJobResponseDTO?> GetByIdAsync(int id)
    {
        var job = await _context.ProcessingJobs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (job == null)
        {
            return null;
        }
        return new ProcessingJobResponseDTO
        {
            Id = job.Id,
            Status = job.Status.ToString(),
            Attempts = job.Attempts,
            ErrorSummary = job.ErrorSummary,
            StartedAt = job.StartedAt,
            CompletedAt = job.CompletedAt,
            CreatedAt = job.CreatedAt,
            CorrelationId = job.CorrelationId
        };
    }
}