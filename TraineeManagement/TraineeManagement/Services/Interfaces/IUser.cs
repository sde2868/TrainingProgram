using TraineeManagement.Models;

namespace TraineeManagement.Interfaces
{
    public interface IUser
    {
        Task<User?> Login(string username, string password);
        Task<PaginationDTO<UserDTO>> GetAllUsers(int pageNumber, int pageSize, string? search, UserRole? role);
        Task<User?> GetUserById(int id);
        Task<User> CreateUser(UserDTO dto);
        Task<User?> UpdateUserById(int id, UserDTO dto);
        Task<User?> DeleteUserById(int id);
        UserDTO ReturnUserDTO(User user);
    }
}