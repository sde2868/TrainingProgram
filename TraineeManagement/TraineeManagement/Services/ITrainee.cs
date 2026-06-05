using TraineeManagement.Models;

namespace TraineeManagement.Services
{
    public interface ITrainee
    {
        List<Trainee> GetAll();
        Trainee GetById(int id);
        Trainee Create(TraineeDTO dto);
        Trainee Put(int id, TraineeDTO dto);
        Trainee DeleteById(int id);

        TraineeDTO ReturnDTO(Trainee tr);
    }
}