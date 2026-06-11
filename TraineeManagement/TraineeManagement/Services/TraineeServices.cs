using TraineeManagement.Models;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Data;

namespace TraineeManagement.Services
{
    public class TraineeServices : ITrainee
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TraineeServices> _logger;
        public TraineeServices(AppDbContext context, ILogger<TraineeServices> logger)
        {

            try
            {
                _context = context;
                _logger = logger;
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
                        Status = TraineeStatus.Active,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });

                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                        "Error while seeding trainee data.");
                throw new Exception($"Error while seeding trainee data.", ex);
            }
        }

        public async Task<PaginationDTO<TraineeDTO>> GetTraineesAsync(int pageNumber, int pageSize, string? search, TraineeStatus? status)
        {
            try
            {
                var query = _context.Trainees.AsQueryable();

                // Search filter
                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();

                    query = query.Where(t =>
                        t.FirstName.ToLower().Contains(search) ||
                        t.LastName.ToLower().Contains(search) ||
                        t.Status.ToString().ToLower().Contains(search) ||
                        // t.TechStack.Any(ts => ts.ToLower().Contains(search)) ||
                        t.Email.ToLower().Contains(search));
                }

                // Status filter

                if (status.HasValue)
                {
                    query = query.Where(t => t.Status.Equals(status.Value));
                }

                // Total count before pagination
                var totalRecords = await query.CountAsync();

                // Pagination using Skip and Take
                var trainees = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(t => new TraineeDTO
                    {
                        // id = t.id,
                        FirstName = t.FirstName,
                        LastName = t.LastName,
                        Email = t.Email,
                        Status = t.Status,
                        TechStack = t.TechStack
                    })
                    .ToListAsync();

                return new PaginationDTO<TraineeDTO>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalRecords,
                    Data = trainees
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                        "Error searching trainees.");
                throw new Exception("Error searching trainees.", ex);
            }
        }

        public async Task<Trainee?> GetById(int id)
        {
            try
            {
                Trainee? trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.id == id);
                if (trainee == null)
                {
                    _logger.LogWarning(
                        "Trainee not found. Id: {Id}",
                        id);
                }
                return trainee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                        "Error while fetching trainee.");
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

                _logger.LogInformation(
                    "Trainee created successfully. Id: {Id}, Email: {Email}",
                    trainee.id,
                    trainee.Email
                );

                return trainee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                        "Error while creating trainee.");

                throw new Exception($"Error while creating trainee.", ex);
            }

        }

        public async Task<Trainee?> Put(int id, TraineeDTO dto)
        {
            try
            {
                Trainee? trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.id == id);

                if (trainee == null)
                {
                    _logger.LogWarning(
                        "Update failed. Trainee not found. Id: {Id}",
                        id);

                    return null;
                }

                trainee.FirstName = dto.FirstName;
                trainee.LastName = dto.LastName;
                trainee.Email = dto.Email;
                trainee.Status = dto.Status;
                trainee.TechStack = dto.TechStack;
                trainee.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Trainee updated successfully. Id: {Id}",
                    trainee.id
                );

                return trainee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                        "Error while updating trainee.");
                throw new Exception($"Error while updating trainee with id {id}.", ex);
            }

        }

        public async Task<Trainee?> DeleteById(int id)
        {
            try
            {
                Trainee? trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.id == id);

                if (trainee == null)
                {
                    _logger.LogWarning(
                        "Delete failed. Trainee not found. Id: {Id}",
                        id);

                    return null;
                }

                _context.Trainees.Remove(trainee);

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Trainee deleted successfully. Id: {Id}",
                    trainee.id
                );

                return trainee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                        "Error while deleting trainee.");
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