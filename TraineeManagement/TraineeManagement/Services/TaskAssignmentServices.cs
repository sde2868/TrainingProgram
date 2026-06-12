using TraineeManagement.Models;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Data;

namespace TraineeManagement.Services
{
    public class TaskAssignmentServices : ITaskAssignment
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TaskAssignmentServices> _logger;
        public TaskAssignmentServices(AppDbContext context, ILogger<TaskAssignmentServices> logger)
        {

            try
            {
                _context = context;
                _logger = logger;
                // // seed data
                // if (!_context.TaskAssignments.Any())
                // {
                //     _context.TaskAssignments.Add(new TaskAssignment
                //     {
                //         Id = 1,
                //         Title = "Zeus",
                //         Description = "Learning",
                //         ExpectedTechStack = ["C#", "Dotnet"],
                //         Status = TaskAssignmentStatus.Draft,
                //         DueDate = DateTime.UtcNow,
                //         CreatedAt = DateTime.UtcNow,
                //         UpdatedAt = DateTime.UtcNow
                //     });

                //     _context.SaveChanges();
                // }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                        "Error while seeding task assignment data.");
                throw new Exception($"Error while seeding task assignment data.", ex);
            }
        }

        public async Task<List<TaskAssignment>> GetAll(string? search)
        {
            try
            {
                List<TaskAssignment> taskAssignments = await _context.TaskAssignments.ToListAsync();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();

                    taskAssignments = taskAssignments.Where(t =>
                        t.Status.ToString().ToLower().Contains(search) ||
                        t.Remarks.ToLower().Contains(search)
                    ).ToList();
                }

                _logger.LogInformation($"task assignments ({taskAssignments.Count}) fetched successfully.");

                return taskAssignments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching task assignments.");
                throw new Exception($"Error while deleting task assignments.", ex);
            }
        }

        public async Task<TaskAssignment?> GetById(int id)
        {
            try
            {
                TaskAssignment? taskAssignment = await _context.TaskAssignments.Include(t => t.Submissions).FirstOrDefaultAsync(t => t.Id == id);
                if (taskAssignment == null)
                {
                    _logger.LogWarning(
                        "TaskAssignment not found. Id: {Id}",
                        id);
                }
                return taskAssignment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                        "Error while fetching task assignment.");
                throw new Exception($"Error while feetching task assignment with id {id}.", ex);
            }

        }

        public async Task<TaskAssignment> Create(TaskAssignmentDTO dto)
        {
            try
            {
                // Validate FK
                if (!await _context.Trainees.AnyAsync(x => x.id == dto.TraineeId))
                    throw new Exception("Trainee does not exist");

                if (!await _context.Mentors.AnyAsync(x => x.Id == dto.MentorId))
                    throw new Exception("Mentor does not exist");

                if (!await _context.LearningTasks.AnyAsync(x => x.Id == dto.LearningTaskId))
                    throw new Exception("LearningTask does not exist");

                // Validate dates
                if (dto.DueDate < dto.AssignedDate)
                    throw new Exception("DueDate cannot be before AssignedDate");

                TaskAssignment taskAssignment = new TaskAssignment
                {
                    Id = _context.TaskAssignments.ToArray().Length == 0 ? 1 : _context.TaskAssignments.ToArray().Length + 1,
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

                _logger.LogInformation(
                    "task assignment created successfully. Id: {Id}",
                    taskAssignment.Id
                );

                return taskAssignment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                        "Error while creating task assignment.");

                throw new Exception($"Error while creating task assignment.", ex);
            }

        }

        public async Task<TaskAssignment?> Put(int id, TaskAssignmentStatus status)
        {
            try
            {
                TaskAssignment? taskAssignment = await _context.TaskAssignments.FirstOrDefaultAsync(t => t.Id == id);

                if (taskAssignment == null)
                {
                    _logger.LogWarning(
                        "Update failed. Task assignment not found. Id: {Id}",
                        id);

                    return null;
                }

                // if (!await _context.Trainees.AnyAsync(x => x.id == dto.TraineeId))
                //     throw new Exception("Trainee does not exist");

                // if (!await _context.Mentors.AnyAsync(x => x.Id == dto.MentorId))
                //     throw new Exception("Mentor does not exist");

                // if (!await _context.LearningTasks.AnyAsync(x => x.Id == dto.LearningTaskId))
                //     throw new Exception("LearningTask does not exist");

                // if (dto.DueDate < dto.AssignedDate)
                //     throw new Exception("DueDate cannot be before AssignedDate");

                // taskAssignment.TraineeId = dto.TraineeId;
                // taskAssignment.MentorId = dto.MentorId;
                // taskAssignment.LearningTaskId = dto.LearningTaskId;
                // taskAssignment.Status = dto.Status;
                // taskAssignment.AssignedDate = dto.AssignedDate;
                // taskAssignment.DueDate = dto.DueDate;
                // taskAssignment.Remarks = dto.Remarks;
                taskAssignment.Status = status;

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "task assignment updated successfully. Id: {Id}",
                    taskAssignment.Id
                );

                return taskAssignment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                        "Error while updating task assignment.");
                throw new Exception($"Error while updating task assignment with id {id}.", ex);
            }

        }

        public TaskAssignmentDTO ReturnDTO(TaskAssignment t)
        {
            try
            {
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
                throw new Exception("Error while converting task assignment to DTO.", ex);
            }

        }
    }
}