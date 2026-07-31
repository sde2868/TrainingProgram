using TraineeManagement.Models;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Data;
using TraineeManagement.Interfaces;
using TraineeManagement.Constants;

namespace TraineeManagement.Services
{
    public class TaskAssignmentServices : ITaskAssignment
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TaskAssignmentServices> _logger;
        private readonly ICacheService _cache;
        public TaskAssignmentServices(AppDbContext context, ILogger<TaskAssignmentServices> logger, ICacheService cache)
        {

            try
            {
                _context = context;
                _logger = logger;
                _cache = cache;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while seeding task assignment data.");
                throw;
            }
        }

        public async Task<PaginationDTO<TaskAssignmentDTO>> GetAllTaskAssignments(int pageNumber = 1, int pageSize = 10, string? search = null, TaskAssignmentStatus? status = null)
        {
            try
            {
                _logger.LogInformation($"GetAllTaskAssignments: filtering all the task assignments by search {search}.");
                var query = _context.TaskAssignments.AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();

                    query = query.Where(t =>
                        t.Remarks.ToLower().Contains(search)
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
                var taskAssignemnts = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(t => new TaskAssignmentDTO
                    {
                        // id = t.id,
                        TraineeId = t.TraineeId,
                        MentorId = t.MentorId,
                        LearningTaskId = t.LearningTaskId,
                        AssignedDate = t.AssignedDate,
                        DueDate = t.DueDate,
                        Status = t.Status,
                        Remarks = t.Remarks
                    })
                    .ToListAsync();

                _logger.LogInformation($"GetAllTaskAssignments: task assignments ({totalRecords}) fetched successfully for search {search}.");
                return new PaginationDTO<TaskAssignmentDTO>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalRecords,
                    Data = taskAssignemnts
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetAllTaskAssignments: error fetching task assignments for search {search}.");
                throw;
            }
        }

        public async Task<TaskAssignment?> GetTaskAssignmentById(int id)
        {
            string cacheKey = CacheKeys.TaskAssignment(id);
            try
            {
                _logger.LogInformation($"GetTaskAssignemntById: fetching task assignment with id {id}.");
                TaskAssignment? cached = await _cache.GetAsync<TaskAssignment>(cacheKey);
                if (cached != null)
                {
                    _logger.LogInformation($"GetTaskAssignemntById: returned task assignment {id} from cache.");
                    return cached;
                }

                TaskAssignment? taskAssignment = await _context.TaskAssignments
                    .AsNoTracking()
                    // .Include(t => t.Submissions)
                    .FirstOrDefaultAsync(t => t.Id == id);
                if (taskAssignment == null)
                {
                    _logger.LogWarning($"GetTaskAssignemntById: task assignment not found. Id: {id}");
                    return null;
                }

                await _cache.SetAsync(cacheKey, taskAssignment);
                return taskAssignment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetTaskAssignemntById: error while fetching task assignment with id {id}.");
                throw;
            }

        }

        public async Task<TaskAssignment> CreateTaskAssignment(TaskAssignmentDTO dto)
        {
            try
            {
                _logger.LogInformation($"Creating new task assignment.");
                // Validate FK
                if (!await _context.Trainees.AnyAsync(x => x.id == dto.TraineeId))
                    throw new KeyNotFoundException("Trainee does not exist");

                if (!await _context.Mentors.AnyAsync(x => x.Id == dto.MentorId))
                    throw new KeyNotFoundException("Mentor does not exist");

                if (!await _context.LearningTasks.AnyAsync(x => x.Id == dto.LearningTaskId))
                    throw new KeyNotFoundException("LearningTask does not exist");

                // Validate dates
                if (dto.DueDate < dto.AssignedDate)
                    throw new KeyNotFoundException("DueDate cannot be before AssignedDate");

                TaskAssignment taskAssignment = new TaskAssignment
                {
                    TraineeId = dto.TraineeId,
                    MentorId = dto.MentorId,
                    LearningTaskId = dto.LearningTaskId,
                    Status = dto.Status,
                    AssignedDate = dto.AssignedDate,
                    DueDate = dto.DueDate,
                    Remarks = dto.Remarks
                };

                await _context.TaskAssignments.AddAsync(taskAssignment);

                await _context.SaveChangesAsync();

                _logger.LogInformation($"CreateTaskAssignment: task assignment created successfully. Id: {taskAssignment.Id}");

                return taskAssignment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateTaskAssignment: error while creating task assignment.");
                throw;
            }

        }

        public async Task<TaskAssignment?> UpdateTaskAssignmentStatus(int id, TaskAssignmentStatus status)
        {
            string cacheKey = CacheKeys.TaskAssignment(id);
            try
            {
                _logger.LogInformation($"UpdateTaskAssignmentStatus: updating status of task assignment id: {id}.");
                TaskAssignment? taskAssignment = await _context.TaskAssignments.FirstOrDefaultAsync(t => t.Id == id);

                if (taskAssignment == null)
                {
                    _logger.LogWarning($"UpdateTaskAssignmentStatus: update failed. Task assignment not found. Id: {id}");
                    await _cache.RemoveAsync(cacheKey);
                    return null;
                }

                taskAssignment.Status = status;

                await _context.SaveChangesAsync();
                await _cache.RemoveAsync(cacheKey);

                _logger.LogInformation($"UpdateTaskAssignmentStatus: task assignment updated successfully. Id: {id}");
                return taskAssignment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UpdateTaskAssignmentStatus: error while updating task assignment with id {id}.");
                throw;
            }
        }

        public TaskAssignmentDTO ReturnTaskAssignmentDTO(TaskAssignment t)
        {
            try
            {
                _logger.LogInformation($"ReturnTaskAssignmentDTO: converting review with id {t.Id} to DTO.");
                TaskAssignmentDTO dto = new()
                {
                    TraineeId = t.TraineeId,
                    MentorId = t.MentorId,
                    LearningTaskId = t.LearningTaskId,
                    Status = t.Status,
                    AssignedDate = t.AssignedDate,
                    DueDate = t.DueDate,
                    Remarks = t.Remarks
                };
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError($"ReturnTaskAssignmentDTO: error converting review with id {t.Id} to DTO.");
                throw;
            }
        }
    }
}