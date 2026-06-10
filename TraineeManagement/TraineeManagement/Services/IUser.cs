using TraineeManagement.Models;

namespace TraineeManagement.Services
{
    public interface IUser
    {
        Task<User?> Login(string username, string password);
        Task<List<User>> GetAll(string? search);
        Task<User?> GetById(int id);
        Task<User> Create(UserDTO dto);
        Task<User?> Put(int id, UserDTO dto);
        Task<User?> DeleteById(int id);
        UserDTO ReturnDTO(User user);
    }
}