using TraineeManagement.Models;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Data;
using TraineeManagement.Interfaces;

namespace TraineeManagement.Services
{
    public class UserServices : IUser
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserServices> _logger;
        public UserServices(AppDbContext context, ILogger<UserServices> logger)
        {
            try
            {
                _context = context;
                _logger = logger;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while seeding user data.");
                // throw new Exception($"Error while seeding user data.", ex);
                throw;
            }
        }

        public async Task<User?> Login(string username, string password)
        {
            try
            {
                _logger.LogInformation($"Login: user with username {username} logging in...");
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
                if (user == null)
                    return null;

                bool validPassword = BCrypt.Net.BCrypt.Verify(password, user.Password);
                if (!validPassword)
                {
                    _logger.LogInformation("Login: Invalid Password. UserName: {UserName}", username);
                    return null;
                }

                _logger.LogInformation(
                    "Login: User logged in successfully. UserName: {UserName}",
                    user.UserName
                );

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    "Login: Failed login attempt. UserName: {UserName}",
                    username
                );
                // throw new Exception("Error during login.", ex);
                throw;
            }
        }

        public async Task<PaginationDTO<UserDTO>> GetAllUsers(int pageNumber, int pageSize, string? search, UserRole? role)
        {
            try
            {
                _logger.LogInformation($"GetAllUsers: filtering users by search {search}.");
                var query = _context.Users.AsQueryable();

                // Search filter
                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();

                    query = query.Where(u =>
                        u.UserName.ToLower().Contains(search) ||
                        u.Email.ToLower().Contains(search)
                    // u.Role.ToLower().Contains(search)
                    );
                }

                // Role filter
                if (role.HasValue)
                {
                    query = query.Where(u => u.Role.Equals(role.Value));
                }

                var totalRecords = await query.CountAsync();

                var users = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(u => new UserDTO
                    {
                        // id = u.id,
                        UserName = u.UserName,
                        Email = u.Email,
                        Role = u.Role,
                        Password = u.Password
                    })
                    .ToListAsync();

                _logger.LogInformation($"GetAllUsers: successfully fetched {totalRecords} users.");
                return new PaginationDTO<UserDTO>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalRecords,
                    Data = users
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetAllUsers: error fetching users with search {search}.");
                // throw new Exception($"Error while fetcing users.", ex);
                throw;
            }
        }

        public async Task<User?> GetUserById(int id)
        {
            try
            {
                _logger.LogInformation($"GetUserById: fetching user with id {id}.");
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if(user == null)
                {
                    _logger.LogWarning($"GetUserById: user with id {id} not found.");
                    return null;
                }
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetUserById: error fetching user with id {id}.");
                // throw new Exception($"Error while fetching user wih id {id}.", ex);
                throw;
            }
        }

        public async Task<User> CreateUser(UserDTO dto)
        {
            try
            {
                _logger.LogInformation($"CreateUser: creating new user.");
                User user = new User
                {
                    // Id = _context.Users.ToArray().Length == 0 ? 1 : _context.Users.ToArray().Length + 1,
                    UserName = dto.UserName,
                    Email = dto.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    Role = dto.Role,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"CreateUser: successfully created new user with id {user.Id}.");
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"CreateUser: error creating new user.");
                // throw new Exception($"Error while creating user.", ex);
                throw;
            }
        }

        public async Task<User?> UpdateUserById(int id, UserDTO dto)
        {
            try
            {
                _logger.LogInformation($"UpdateUser: updating user with id {id}.");
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    _logger.LogInformation($"UpdateUser: user with id {id} not found.");
                    return null;
                }

                user.UserName = dto.UserName;
                user.Email = dto.Email;
                user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
                user.Role = dto.Role;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                _logger.LogInformation($"UpdateUser: user eith id {id} updated successfully.");
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"UpdateUser: error updating user with id {id}.");
                // throw new Exception($"Error updating user with id {id}.", ex);
                throw;
            }
        }

        public async Task<User?> DeleteUserById(int id)
        {
            try
            {
                _logger.LogInformation($"DeleteUserById: deleting user by id {id}.");
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    _logger.LogWarning($"DeleteUserById: user with id {id} not found.");
                    return null;
                }
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"DeleteUserById: successfully deleted user with id {id}.");
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"DeleteUserById: error deleting user with id {id}.");
                // throw new Exception($"Error deleting user with id {id}.", ex);
                throw;
            }
        }

        public UserDTO ReturnUserDTO(User user)
        {
            try
            {
                _logger.LogInformation($"ReturnUserDTO: converting user with id {user.Id} to DTO.");
                UserDTO dto = new()
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Role = user.Role
                };
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError($"ReturnUserDTO: error converting user with id {user.Id} to DTO.");
                // throw new Exception("Error while converting user to DTO.", ex);
                throw;
            }
        }
    }
}