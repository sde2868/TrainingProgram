using TraineeManagement.Models;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Data;
using TraineeManagement.Constants;
using TraineeManagement.Interfaces;

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
                _logger.LogError(ex, "Error while seeding trainee data.");
                throw new Exception($"Error while seeding trainee data.", ex);
            }
        }

        public async Task<PaginationDTO<TraineeDTO>> GetAllTrainees(int pageNumber, int pageSize, string? search, TraineeStatus? status)
        {
            try
            {
                _logger.LogInformation($"GetAllTrainees: filtering trainees by search {search}.");
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

                _logger.LogInformation($"GetAllTrainees: successfully fetched {totalRecords} trainees by search {search}.");
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
                _logger.LogError(ex, $"GetAllTrainees: error searching trainees with search {search}.");
                throw new Exception("Error searching trainees.", ex);
            }
        }

        public async Task<Trainee?> GetTraineeById(int id)
        {
            try
            {
                _logger.LogInformation($"GetTraineeById: fetching trainee by id {id}.");
                Trainee? trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.id == id);
                if (trainee == null)
                {
                    _logger.LogWarning($"GetTraineeById: trainee not found. Id: {id}");
                    return null;
                }
                return trainee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetTraineeById: error while fetching trainee with id {id}.");
                throw new Exception($"Error while feetching trainee with id {id}.", ex);
            }

        }

        public async Task<Trainee> CreateTrainee(TraineeDTO dto)
        {
            try
            {
                _logger.LogInformation("CreateTrainee: creating new trainee.");
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
                    "CreateTrainee: trainee created successfully. Id: {Id}, Email: {Email}",
                    trainee.id,
                    trainee.Email
                );
                return trainee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateTrainee: error while creating trainee.");
                throw new Exception($"Error while creating trainee.", ex);
            }

        }

        public async Task<Trainee?> UpdateTrainee(int id, TraineeDTO dto)
        {
            try
            {
                _logger.LogInformation($"UpdateTrainee: updating trainee with id {id}.");
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

                _logger.LogInformation($"UpdateTrainee: trainee updated successfully. Id: {id}");

                return trainee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UpdateTrainee: error while updating trainee with id {id}.");
                throw new Exception($"Error while updating trainee with id {id}.", ex);
            }

        }

        public async Task<Trainee?> DeleteTraineeById(int id)
        {
            try
            {
                _logger.LogInformation($"DeleteTraineeById: deleting trainee with id {id}.");
                Trainee? trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.id == id);

                if (trainee == null)
                {
                    _logger.LogWarning($"DeleteTraineeById: trainee not found. Id: {id}.");
                    return null;
                }

                _context.Trainees.Remove(trainee);

                await _context.SaveChangesAsync();

                _logger.LogInformation($"DeleteTraineeById: trainee deleted successfully. Id: {id}");

                return trainee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DeleteTraineeById: error while deleting trainee with id {id}.");
                throw new Exception($"Error while deleting trainee with id {id}.", ex);
            }

        }

        public TraineeDTO ReturnTraineeDTO(Trainee tr)
        {
            try
            {
                _logger.LogInformation($"ReturnTraineeDTO: converting trainee with id {tr.id} to DTO.");
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
                _logger.LogInformation($"ReturnTraineeDTO: converting trainee with id {tr.id} to DTO.");
                throw new Exception("Error while converting trainee to DTO.", ex);
            }

        }
    }
}