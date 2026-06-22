using TraineeManagement.Models;

namespace TraineeManagement.Interfaces
{
    public interface ITaskSubmission
    {
        public Task<PaginationDTO<TaskSubmissionDTO>> GetAllTaskSubmissions(int pageNumber = 1, int pageSize = 10, string? search = null, TaskSubmissionStatus? status = null);
        public Task<TaskSubmission?> GetTaskSubmissionById(int id);
        public Task<TaskSubmission> CreateTaskSubmission(TaskSubmissionDTO dto);
        TaskSubmissionDTO ReturnTaskSubmissionDTO(TaskSubmission tasksubmission);
    }
}