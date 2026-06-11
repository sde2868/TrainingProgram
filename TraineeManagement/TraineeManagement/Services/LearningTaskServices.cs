using TraineeManagement.Models;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Data;

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
                _logger.LogError(ex,
                        "Error while seeding learning task data.");
                throw new Exception($"Error while seeding learning task data.", ex);
            }
        }

        public async Task<List<LearningTask>> GetAll(string? search)
        {
            try
            {
                List<LearningTask> learningTasks = await _context.LearningTasks.ToListAsync();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();

                    learningTasks = learningTasks.Where(t =>
                        t.Title.ToLower().Contains(search) ||
                        t.Description.ToLower().Contains(search) ||
                        t.Status.ToString().ToLower().Contains(search) ||
                        t.ExpectedTechStack.Any(ts => ts.ToLower().Contains(search)) ||
                        t.DueDate.ToLocalTime().ToString().ToLower().Contains(search)
                    ).ToList();
                }

                _logger.LogInformation($"Learning tasks ({learningTasks.Count}) fetched successfully.");

                return learningTasks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching learning tasks.");
                throw new Exception($"Error while deleting learning tasks.", ex);
            }
        }

        public async Task<LearningTask?> GetById(int id)
        {
            try
            {
                LearningTask? learningTask = await _context.LearningTasks.FirstOrDefaultAsync(t => t.Id == id);
                if (learningTask == null)
                {
                    _logger.LogWarning(
                        "LearningTask not found. Id: {Id}",
                        id);
                }
                return learningTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                        "Error while fetching learning task.");
                throw new Exception($"Error while feetching learning task with id {id}.", ex);
            }

        }

        public async Task<LearningTask> Create(LearningTaskDTO dto)
        {
            try
            {
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
                    "Learning task created successfully. Id: {Id}, Title: {Title}",
                    learningTask.Id,
                    learningTask.Title
                );

                return learningTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                        "Error while creating learning task.");

                throw new Exception($"Error while creating learning task.", ex);
            }

        }

        public async Task<LearningTask?> Put(int id, LearningTaskDTO dto)
        {
            try
            {
                LearningTask? learningTask = await _context.LearningTasks.FirstOrDefaultAsync(t => t.Id == id);

                if (learningTask == null)
                {
                    _logger.LogWarning(
                        "Update failed. Learning task not found. Id: {Id}",
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
                _logger.LogError(ex,
                        "Error while updating learning task.");
                throw new Exception($"Error while updating learning task with id {id}.", ex);
            }

        }

        public async Task<LearningTask?> DeleteById(int id)
        {
            try
            {
                LearningTask? learningTask = await _context.LearningTasks.FirstOrDefaultAsync(t => t.Id == id);

                if (learningTask == null)
                {
                    _logger.LogWarning(
                        "Delete failed. Learning task not found. Id: {Id}",
                        id);

                    return null;
                }

                _context.LearningTasks.Remove(learningTask);

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "LearningTask deleted successfully. Id: {Id}",
                    learningTask.Id
                );

                return learningTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                        "Error while deleting learning task.");
                throw new Exception($"Error while deleting learning task with id {id}.", ex);
            }

        }

        public LearningTaskDTO ReturnDTO(LearningTask t)
        {
            try
            {
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
                throw new Exception("Error while converting learning task to DTO.", ex);
            }

        }
    }
}