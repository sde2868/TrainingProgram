using TraineeManagement.Models;

namespace TraineeManagement.Interfaces
{
    public interface ILearningTask
    {
        Task<PaginationDTO<LearningTaskDTO>> GetAllLearningTasks(int pageNumber, int pageSize, string? search, LearningTaskStatus? status);
        Task<LearningTask?> GetLearningTaskById(int id);
        Task<LearningTask> CreateLearningTask(LearningTaskDTO dto);
        Task<LearningTask?> UpdateLearningTaskById(int id, LearningTaskDTO dto);
        Task<LearningTask?> DeleteLearningById(int id);
        LearningTaskDTO ReturnLearningTaskDTO(LearningTask learningtask);
    }
}