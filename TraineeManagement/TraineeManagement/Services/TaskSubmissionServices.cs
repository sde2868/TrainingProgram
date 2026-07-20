using TraineeManagement.Models;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Data;
using TraineeManagement.Interfaces;
using TraineeManagement.Constants;

namespace TraineeManagement.Services
{
    public class TaskSubmissionServices : ITaskSubmission
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TaskSubmissionServices> _logger;
        private readonly ICacheService _cache;
        public TaskSubmissionServices(AppDbContext context, ILogger<TaskSubmissionServices> logger, ICacheService cache)
        {

            try
            {
                _context = context;
                _logger = logger;
                _cache = cache;
                // // seed data
                // if (!_context.TaskSubmissions.Any())
                // {
                //     _context.TaskSubmissions.Add(new TaskSubmission
                //     {
                //         Id = 1,
                //         Title = "Zeus",
                //         Description = "Learning",
                //         ExpectedTechStack = ["C#", "Dotnet"],
                //         Status = TaskSubmissionStatus.Draft,
                //         DueDate = DateTime.UtcNow,
                //         CreatedAt = DateTime.UtcNow,
                //         UpdatedAt = DateTime.UtcNow
                //     });

                //     _context.SaveChanges();
                // }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while seeding task submission data.");
                // throw new Exception($"Error while seeding task submission data.", ex);
                throw;
            }
        }

        public async Task<PaginationDTO<TaskSubmissionDTO>> GetAllTaskSubmissions(int pageNumber = 1, int pageSize = 10, string? search = null, TaskSubmissionStatus? status = null)
        {
            try
            {
                _logger.LogInformation($"GetAllTaskSubmissions: filtering taks submissions by search {search}.");
                var query = _context.TaskSubmissions.AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();

                    query = query.Where(t =>
                        t.SubmissionUrl.ToLower().Contains(search) ||
                        t.Notes.ToLower().Contains(search)
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
                var taskSubmissions = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(t => new TaskSubmissionDTO
                    {
                        // id = t.id,
                        SubmissionUrl = t.SubmissionUrl,
                        Notes = t.Notes,
                        SubmittedDate = t.SubmittedDate,
                        Status = t.Status,
                    })
                    .ToListAsync();

                _logger.LogInformation($"GetAllTaskSubmissions: task submissions ({totalRecords}) fetched successfully.");

                return new PaginationDTO<TaskSubmissionDTO>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalRecords,
                    Data = taskSubmissions
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetAllTaskSubmissions: error fetching task submissions with search {search}.");
                // throw new Exception($"Error while deleting task submissions.", ex);
                throw;
            }
        }

        public async Task<TaskSubmission?> GetTaskSubmissionById(int id)
        {
            string cacheKey = CacheKeys.TaskSubmission(id);
            try
            {
                _logger.LogInformation($"GetTaskSubmissionById: fetching task submission by id {id}.");
                TaskSubmission? cached = await _cache.GetAsync<TaskSubmission>(cacheKey);
                if (cached != null)
                {
                    _logger.LogInformation($"GetTaskSubmissionById: returned task submission {id} from cache.");
                    return cached;
                }

                TaskSubmission? taskSubmission = await _context.TaskSubmissions
                    .AsNoTracking()
                    // .Include(taskSubmission => taskSubmission.Reviews)
                    .FirstOrDefaultAsync(t => t.Id == id);
                if (taskSubmission == null)
                {
                    _logger.LogWarning($"GetTaskSubmissionById: task submission not found. Id: {id}");
                    return null;
                }

                await _cache.SetAsync(cacheKey, taskSubmission);
                return taskSubmission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetTaskSubmissionById: error while fetching task submission with id {id}.");
                // throw new Exception($"Error while feetching task submission with id {id}.", ex);
                throw;
            }

        }

        public async Task<TaskSubmission> CreateTaskSubmission(TaskSubmissionDTO dto)
        {
            try
            {
                _logger.LogInformation($"CreateTaskSubmission: creating new task submission.");
                // Validate FK
                if (!await _context.TaskAssignments.AnyAsync(x => x.Id == dto.TaskAssignmentId))
                    throw new KeyNotFoundException("TaskAssginment does not exist");

                // Validate dates
                if (dto.SubmittedDate > DateTime.UtcNow)
                    throw new KeyNotFoundException("SubmittedDate cannot be in future");

                TaskSubmission taskSubmission = new TaskSubmission
                {
                    // Id = _context.TaskSubmissions.ToArray().Length == 0 ? 1 : _context.TaskSubmissions.ToArray().Length + 1,
                    TaskAssignmentId = dto.TaskAssignmentId,
                    Status = dto.Status,
                    SubmittedDate = dto.SubmittedDate,
                    SubmissionUrl = dto.SubmissionUrl,
                    Notes = dto.Notes
                };

                await _context.TaskSubmissions.AddAsync(taskSubmission);

                await _context.SaveChangesAsync();

                _logger.LogInformation($"CreateTaskSubmission: task submission created successfully. Id: {taskSubmission.Id}");

                return taskSubmission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateTaskSubmission: error while creating task submission.");
                // throw new Exception($"Error while creating task submission.", ex);
                throw;
            }

        }

        public TaskSubmissionDTO ReturnTaskSubmissionDTO(TaskSubmission t)
        {
            try
            {
                _logger.LogInformation($"ReturnTaskSubmissionDTO: converting task submission with id {t.Id} to DTO.");
                TaskSubmissionDTO dto = new()
                {
                    TaskAssignmentId = t.TaskAssignmentId,
                    Status = t.Status,
                    SubmittedDate = t.SubmittedDate,
                    SubmissionUrl = t.SubmissionUrl,
                    Notes = t.Notes
                };
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError($"ReturnTaskSubmissionDTO: error while converting task submission with id {t.Id} to dto.");
                // throw new Exception("Error while converting task submission to DTO.", ex);
                throw;
            }
        }
    }
}