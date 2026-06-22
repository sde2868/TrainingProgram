using TraineeManagement.Models;

namespace TraineeManagement.Interfaces
{
    public interface ITrainee
    {
        Task<PaginationDTO<TraineeDTO>> GetAllTrainees(int pageNumber, int pageSize, string? search, TraineeStatus? status);
        Task<Trainee?> GetTraineeById(int id);
        Task<Trainee> CreateTrainee(TraineeDTO dto);
        Task<Trainee?> UpdateTrainee(int id, TraineeDTO dto);
        Task<Trainee?> DeleteTraineeById(int id);
        TraineeDTO ReturnTraineeDTO(Trainee tr);
    }
}