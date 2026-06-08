using TraineeManagement.Models;

namespace TraineeManagement.Services
{
    public interface ITrainee
    {
        Task<List<Trainee>> GetAll(string? search);
        Task<Trainee?> GetById(int id);
        Task<Trainee> Create(TraineeDTO dto);
        Task<Trainee?> Put(int id, TraineeDTO dto);
        Task<Trainee?> DeleteById(int id);
        TraineeDTO ReturnDTO(Trainee tr);
    }
}