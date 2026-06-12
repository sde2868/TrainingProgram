using TraineeManagement.Models;

namespace TraineeManagement.Services
{
    public interface ITaskSubmission
    {
        public Task<List<TaskSubmission>> GetAll(string? search);
        public Task<TaskSubmission?> GetById(int id);
        public Task<TaskSubmission> Create(TaskSubmissionDTO dto);
        TaskSubmissionDTO ReturnDTO(TaskSubmission tasksubmission);
    }
}