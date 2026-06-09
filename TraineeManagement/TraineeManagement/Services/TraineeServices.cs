using TraineeManagement.Models;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Data;

namespace TraineeManagement.Services
{
    public class TraineeServices : ITrainee
    {
        private readonly AppDbContext _context;
        public TraineeServices(AppDbContext context)
        {

            try
            {
                _context = context;

                // seed data
                if (!_context.Trainees.Any())
                {
                    _context.Trainees.Add(new Trainee
                    {
                        id = 1,
                        FirstName = "Zeus",
                        LastName = "Learning",
                        Email = "zeuslearning@email.com",
                        TechStack = ["C#", "Dotnet"],
                        Status = "Active",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });

                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while seeding trainee data.", ex);
            }

        }

        public async Task<List<Trainee>> GetAll(string? search)
        {
            try
            {
                List<Trainee> trainees = await _context.Trainees.ToListAsync();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();

                    trainees = trainees.Where(t =>
                        t.FirstName.ToLower().Contains(search) ||
                        t.LastName.ToLower().Contains(search) ||
                        t.Email.ToLower().Contains(search) ||
                        t.TechStack.Any(ts => ts.ToLower().Contains(search))
                    ).ToList();
                }

                return trainees;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while deleting trainees.", ex);
            }
        }

        public async Task<Trainee?> GetById(int id)
        {
            try
            {
                Trainee? trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.id == id);
                return trainee;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while feetching trainee with id {id}.", ex);
            }

        }

        public async Task<Trainee> Create(TraineeDTO dto)
        {
            try
            {
                Trainee trainee = new Trainee
                {
                    id = _context.Trainees.ToArray().Length == 0 ? 1 : _context.Trainees.ToArray().Length + 1,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    TechStack = dto.TechStack,
                    Status = dto.Status,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _context.Trainees.AddAsync(trainee);

                await _context.SaveChangesAsync();

                return trainee;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while creating trainee.", ex);
            }

        }

        public async Task<Trainee?> Put(int id, TraineeDTO dto)
        {
            try
            {
                Trainee? trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.id == id);

                if (trainee == null)
                    return null;

                trainee.FirstName = dto.FirstName;
                trainee.LastName = dto.LastName;
                trainee.Email = dto.Email;
                trainee.Status = dto.Status;
                trainee.TechStack = dto.TechStack;
                trainee.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return trainee;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while updating trainee with id {id}.", ex);
            }

        }

        public async Task<Trainee?> DeleteById(int id)
        {
            try
            {
                Trainee? trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.id == id);

                if (trainee == null)
                    return null;

                _context.Trainees.Remove(trainee);

                await _context.SaveChangesAsync();

                return trainee;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while deleting trainee with id {id}.", ex);
            }

        }

        public TraineeDTO ReturnDTO(Trainee tr)
        {
            try
            {
                TraineeDTO dto = new()
                {
                    FirstName = tr.FirstName,
                    LastName = tr.LastName,
                    Email = tr.Email,
                    TechStack = tr.TechStack,
                    Status = tr.Status
                };
                return dto;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while converting trainee to DTO.", ex);
            }

        }
    }
}