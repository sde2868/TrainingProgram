using TraineeManagement.Models;

namespace TraineeManagement.Services
{
    public interface ILearningTask
    {
        Task<List<LearningTask>> GetAll(string? search);
        Task<LearningTask?> GetById(int id);
        Task<LearningTask> Create(LearningTaskDTO dto);
        Task<LearningTask?> Put(int id, LearningTaskDTO dto);
        Task<LearningTask?> DeleteById(int id);
        LearningTaskDTO ReturnDTO(LearningTask learningtask);
    }
}