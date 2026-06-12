using TraineeManagement.Models;

namespace TraineeManagement.Services
{
    public interface ITaskAssignment
    {
        public Task<List<TaskAssignment>> GetAll(string? search);
        public Task<TaskAssignment?> GetById(int id);
        public Task<TaskAssignment> Create(TaskAssignmentDTO dto);
        public Task<TaskAssignment?> Put(int id, TaskAssignmentStatus status);
        TaskAssignmentDTO ReturnDTO(TaskAssignment taskassignment);
    }
}