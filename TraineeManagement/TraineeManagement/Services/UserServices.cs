using TraineeManagement.Models;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Data;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TraineeManagement.Services
{
    public class UserServices : IUser
    {
        private readonly AppDbContext _context;
        public UserServices(AppDbContext context)
        {
            try
            {
                _context = context;

                // // seed data
                // if (!_context.Users.Any())
                // {
                //     _context.Users.Add(new User
                //     {
                //         Id = 1,
                //         UserName = "Zeus_Learning",
                //         Email = "admin@zeuslearning.com",
                //         Role = ,
                //         CreatedAt = DateTime.UtcNow,
                //         UpdatedAt = DateTime.UtcNow
                //     });
                //     _context.SaveChanges();
                // }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while seeding user data.", ex);
            }
        }

        public async Task<User?> Login(string username, string password)
        {
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
                if (user == null)
                    return null;

                bool validPassword = BCrypt.Net.BCrypt.Verify( password, user.Password );
                if (!validPassword) return null;

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Error during login.", ex);
            }
        }

        public async Task<List<User>> GetAll(string? search)
        {
            try
            {
                List<User> users = await _context.Users.ToListAsync();
                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();

                    users = users.Where(u =>
                        u.UserName.ToLower().Contains(search) ||
                        u.Email.ToLower().Contains(search)
                    // u.Role.ToLower().Contains(search)
                    ).ToList();
                }

                return users;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while fetcing users.", ex);
            }
        }

        public async Task<User?> GetById(int id)
        {
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while fetching user wih id {id}.", ex);
            }
        }

        public async Task<User> Create(UserDTO dto)
        {
            try
            {
                User user = new User
                {
                    Id = _context.Users.ToArray().Length == 0 ? 1 : _context.Users.ToArray().Length + 1,
                    UserName = dto.UserName,
                    Email = dto.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    Role = dto.Role,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while creating user.", ex);
            }
        }

        public async Task<User?> Put(int id, UserDTO dto)
        {
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null) return null;

                user.UserName = dto.UserName;
                user.Email = dto.Email;
                user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
                user.Role = dto.Role;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updsting user with id {id}.", ex);
            }
        }

        public async Task<User?> DeleteById(int id)
        {
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null) return null;

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting user with id {id}.", ex);
            }
        }

        public UserDTO ReturnDTO(User user)
        {
            try
            {
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
                throw new Exception("Error while converting user to DTO.", ex);
            }
        }
    }
}