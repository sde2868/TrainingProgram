using TraineeManagement.Models;

namespace TraineeManagement.Interfaces;
public interface IProcessingJobService
{
    Task<ProcessingJobResponseDTO?> GetByIdAsync(int id);
}