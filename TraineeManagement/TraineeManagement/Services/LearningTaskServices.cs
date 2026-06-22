using TraineeManagement.Models;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Data;
using TraineeManagement.Interfaces;

namespace TraineeManagement.Services
{
    public class LearningTaskServices : ILearningTask
    {
        private readonly AppDbContext _context;
        private readonly ILogger<LearningTaskServices> _logger;
        public LearningTaskServices(AppDbContext context, ILogger<LearningTaskServices> logger)
        {

            try
            {
                _context = context;
                _logger = logger;
                // seed data
                if (!_context.LearningTasks.Any())
                {
                    _context.LearningTasks.Add(new LearningTask
                    {
                        Id = 1,
                        Title = "Zeus",
                        Description = "Learning",
                        ExpectedTechStack = ["C#", "Dotnet"],
                        Status = LearningTaskStatus.Draft,
                        DueDate = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });

                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while seeding learning task data.");
                throw new Exception($"Error while seeding learning task data.", ex);
            }
        }

        public async Task<PaginationDTO<LearningTaskDTO>> GetAllLearningTasks(int pageNumber, int pageSize, string? search, LearningTaskStatus? status)
        {
            try
            {
                _logger.LogInformation($"GetAllLearningTasks: Searching for {search} in all learning tasks.");
                var query = _context.LearningTasks.AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();

                    query = query.Where(t =>
                        t.Title.ToLower().Contains(search) ||
                        t.Description.ToLower().Contains(search) ||
                        t.ExpectedTechStack.Any(ts => ts.ToLower().Contains(search)) ||
                        t.DueDate.ToLocalTime().ToString().ToLower().Contains(search)
                    );
                }

                // Status filter
                if (status.HasValue)
                {
                    query = query.Where(t => t.Status.Equals(status.Value));
                }

                // Total count before pagination
                var totalRecords = await query.CountAsync();

                // Pagination using Skip and Take
                var learningTasks = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(t => new LearningTaskDTO
                    {
                        // id = t.id,
                        Title = t.Title,
                        Description = t.Description,
                        ExpectedTechStack = t.ExpectedTechStack,
                        Status = t.Status,
                        DueDate = t.DueDate
                    })
                    .ToListAsync();

                _logger.LogInformation($"GetAllLearnnigTasks: Learning tasks ({totalRecords}) fetched successfully for search {search}.");
                return new PaginationDTO<LearningTaskDTO>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalRecords,
                    Data = learningTasks
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetAllLearningTasks: Error fetching learning tasks for search {search}.");
                throw new Exception($"Error while deleting learning tasks.", ex);
            }
        }

        public async Task<LearningTask?> GetLearningTaskById(int id)
        {
            try
            {
                _logger.LogInformation($"GetLearningTaskById: fetching learning task id: {id}.");
                LearningTask? learningTask = await _context.LearningTasks.FirstOrDefaultAsync(t => t.Id == id);
                if (learningTask == null)
                {
                    _logger.LogWarning("GetLearningTaskById: LearningTask not found. Id: {Id}", id);
                }
                return learningTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetLearningTaskById, Error while fetching learning task by id {id}.");
                throw new Exception($"Error while fetching learning task with id {id}.", ex);
            }

        }

        public async Task<LearningTask> CreateLearningTask(LearningTaskDTO dto)
        {
            try
            {
                _logger.LogInformation($"CreateLearningTask: creating new learning task.");
                LearningTask learningTask = new LearningTask
                {
                    Id = _context.LearningTasks.ToArray().Length == 0 ? 1 : _context.LearningTasks.ToArray().Length + 1,
                    Title = dto.Title,
                    Description = dto.Description,
                    ExpectedTechStack = dto.ExpectedTechStack,
                    Status = dto.Status,
                    DueDate = dto.DueDate,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _context.LearningTasks.AddAsync(learningTask);

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "CreateLearningTask: Learning task created successfully. Id: {Id}, Title: {Title}",
                    learningTask.Id,
                    learningTask.Title
                );

                return learningTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateLeaningTask: Error while creating learning task.");
                throw new Exception($"Error while creating learning task.", ex);
            }

        }

        public async Task<LearningTask?> UpdateLearningTaskById(int id, LearningTaskDTO dto)
        {
            try
            {
                _logger.LogInformation($"UpdateLearningTaskById: updating learning task by if {id}.");
                LearningTask? learningTask = await _context.LearningTasks.FirstOrDefaultAsync(t => t.Id == id);

                if (learningTask == null)
                {
                    _logger.LogWarning(
                        "UpdateLearningTaskById: Update failed. Learning task not found. Id: {Id}",
                        id);

                    return null;
                }

                learningTask.Title = dto.Title;
                learningTask.Description = dto.Description;
                learningTask.Status = dto.Status;
                learningTask.DueDate = dto.DueDate;
                learningTask.ExpectedTechStack = dto.ExpectedTechStack;
                learningTask.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Learning task updated successfully. Id: {Id}",
                    learningTask.Id
                );

                return learningTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while updating learning task with id {id}.");
                throw new Exception($"Error while updating learning task with id {id}.", ex);
            }

        }

        public async Task<LearningTask?> DeleteLearningById(int id)
        {
            try
            {
                _logger.LogInformation($"DeleteLearningTaskById: deleting learning task with id {id}.");
                LearningTask? learningTask = await _context.LearningTasks.FirstOrDefaultAsync(t => t.Id == id);

                if (learningTask == null)
                {
                    _logger.LogWarning($"DeleteLearningTaskById: learning task with id {id} not found.");
                    return null;
                }

                _context.LearningTasks.Remove(learningTask);

                await _context.SaveChangesAsync();

                _logger.LogInformation($"DeleteLearningTaskById: learning task with id {id} deleted successfully.");

                return learningTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DeleteLearningTaskById: error while deleting learning task with id {id}.");
                throw new Exception($"Error while deleting learning task with id {id}.", ex);
            }

        }

        public LearningTaskDTO ReturnLearningTaskDTO(LearningTask t)
        {
            try
            {
                _logger.LogInformation($"ReturnLearningTaskDTO: converting learning task with id {t.Id} to DTO.");
                LearningTaskDTO dto = new()
                {
                    Title = t.Title,
                    Description = t.Description,
                    ExpectedTechStack = t.ExpectedTechStack,
                    Status = t.Status,
                    DueDate = t.DueDate
                };
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ReturnLearningTaskDTO: error while converting learning task with id {t.Id} to DTO");
                throw new Exception("Error while converting learning task to DTO.", ex);
            }

        }
    }
}