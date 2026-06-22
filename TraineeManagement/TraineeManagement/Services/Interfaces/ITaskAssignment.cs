using TraineeManagement.Models;

namespace TraineeManagement.Interfaces
{
    public interface ITaskAssignment
    {
        public Task<PaginationDTO<TaskAssignmentDTO>> GetAllTaskAssignments(int pageNumber = 1, int pageSize = 10, string? search = null, TaskAssignmentStatus? status = null);
        public Task<TaskAssignment?> GetTaskAssignmentById(int id);
        public Task<TaskAssignment> CreateTaskAssignment(TaskAssignmentDTO dto);
        public Task<TaskAssignment?> UpdateTaskAssignmentStatus(int id, TaskAssignmentStatus status);
        TaskAssignmentDTO ReturnTaskAssignmentDTO(TaskAssignment taskassignment);
    }
}