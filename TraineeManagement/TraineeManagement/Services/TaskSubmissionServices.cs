using TraineeManagement.Models;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Data;

namespace TraineeManagement.Services
{
    public class TaskSubmissionServices : ITaskSubmission
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TaskSubmissionServices> _logger;
        public TaskSubmissionServices(AppDbContext context, ILogger<TaskSubmissionServices> logger)
        {

            try
            {
                _context = context;
                _logger = logger;
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
                _logger.LogError(ex,
                        "Error while seeding task submission data.");
                throw new Exception($"Error while seeding task submission data.", ex);
            }
        }

        public async Task<List<TaskSubmission>> GetAll(string? search)
        { // TaskAssignmentId SubmissionUrl Notes SubmittedDate Status
            try
            {
                List<TaskSubmission> taskSubmissions = await _context.TaskSubmissions.ToListAsync();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();

                    taskSubmissions = taskSubmissions.Where(t =>
                        t.Status.ToString().ToLower().Contains(search) ||
                        t.SubmissionUrl.ToLower().Contains(search) ||
                        t.Notes.ToLower().Contains(search)
                    ).ToList();
                }

                _logger.LogInformation($"task submissions ({taskSubmissions.Count}) fetched successfully.");

                return taskSubmissions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching task submissions.");
                throw new Exception($"Error while deleting task submissions.", ex);
            }
        }

        public async Task<TaskSubmission?> GetById(int id)
        {
            try
            {
                TaskSubmission? taskSubmission = await _context.TaskSubmissions.Include(taskSubmission => taskSubmission.Reviews).FirstOrDefaultAsync(t => t.Id == id);
                if (taskSubmission == null)
                {
                    _logger.LogWarning(
                        "TaskSubmission not found. Id: {Id}",
                        id);
                }
                return taskSubmission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                        "Error while fetching task submission.");
                throw new Exception($"Error while feetching task submission with id {id}.", ex);
            }

        }

        public async Task<TaskSubmission> Create(TaskSubmissionDTO dto)
        {
            try
            {
                // Validate FK
                if (!await _context.TaskAssignments.AnyAsync(x => x.Id == dto.TaskAssignmentId))
                    throw new Exception("TaskAssginment does not exist");

                // Validate dates
                if (dto.SubmittedDate > DateTime.UtcNow)
                    throw new Exception("SubmittedDate cannot be in future");

                TaskSubmission taskSubmission = new TaskSubmission
                {
                    Id = _context.TaskSubmissions.ToArray().Length == 0 ? 1 : _context.TaskSubmissions.ToArray().Length + 1,
                    TaskAssignmentId = dto.TaskAssignmentId,
                    Status = dto.Status,
                    SubmittedDate = dto.SubmittedDate,
                    SubmissionUrl = dto.SubmissionUrl,
                    Notes = dto.Notes
                };

                await _context.TaskSubmissions.AddAsync(taskSubmission);

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "task submission created successfully. Id: {Id}",
                    taskSubmission.Id
                );

                return taskSubmission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                        "Error while creating task submission.");

                throw new Exception($"Error while creating task submission.", ex);
            }

        }

        public TaskSubmissionDTO ReturnDTO(TaskSubmission t)
        {
            try
            {
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
                throw new Exception("Error while converting task submission to DTO.", ex);
            }

        }
    }
}