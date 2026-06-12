using TraineeManagement.Models;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Data;

namespace TraineeManagement.Services
{
    public class ReviewServices : IReview
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ReviewServices> _logger;
        public ReviewServices(AppDbContext context, ILogger<ReviewServices> logger)
        {

            try
            {
                _context = context;
                _logger = logger;
                // // seed data
                // if (!_context.Reviews.Any())
                // {
                //     _context.Reviews.Add(new Review
                //     {
                //         Id = 1,
                //         Title = "Zeus",
                //         Description = "Learning",
                //         ExpectedTechStack = ["C#", "Dotnet"],
                //         Status = ReviewStatus.Draft,
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
                        "Error while seeding review data.");
                throw new Exception($"Error while seeding review data.", ex);
            }
        }

        public async Task<List<Review>> GetAll(string? search)
        { // SubmissionId MentorId Feedback Score Status ReviewedDate
            try
            {
                List<Review> reviews = await _context.Reviews.ToListAsync();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();

                    reviews = reviews.Where(r =>
                        r.Status.ToString().ToLower().Contains(search) ||
                        r.Feedback.ToLower().Contains(search)
                    ).ToList();
                }

                _logger.LogInformation($"reviews ({reviews.Count}) fetched successfully.");

                return reviews;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching reviews.");
                throw new Exception($"Error while deleting reviews.", ex);
            }
        }

        public async Task<Review?> GetById(int id)
        {
            try
            {
                Review? review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
                if (review == null)
                {
                    _logger.LogWarning(
                        "Review not found. Id: {Id}",
                        id);
                }
                return review;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                        "Error while fetching review.");
                throw new Exception($"Error while feetching review with id {id}.", ex);
            }

        }

        public async Task<Review> Create(ReviewDTO dto)
        {
            try
            {
                // Validate FK
                if (!await _context.TaskSubmissions.AnyAsync(x => x.Id == dto.TaskSubmissionId))
                    throw new Exception("TaskSubmission does not exist");

                if (!await _context.Mentors.AnyAsync(x => x.Id == dto.MentorId))
                    throw new Exception("Mentor does not exist");    

                // Validate dates
                if (dto.ReviewedDate > DateTime.UtcNow)
                    throw new Exception("ReviewedDate cannot be in future");

                Review review = new Review
                {
                    Id = _context.Reviews.ToArray().Length == 0 ? 1 : _context.Reviews.ToArray().Length + 1,
                    TaskSubmissionId = dto.TaskSubmissionId,
                    MentorId = dto.MentorId,
                    Feedback = dto.Feedback,
                    Score = dto.Score,
                    Status = dto.Status,
                    ReviewedDate = dto.ReviewedDate,
                };

                await _context.Reviews.AddAsync(review);

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "review created successfully. Id: {Id}",
                    review.Id
                );

                return review;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                        "Error while creating review.");

                throw new Exception($"Error while creating review.", ex);
            }

        }

        public ReviewDTO ReturnDTO(Review r)
        {
            try
            {
                ReviewDTO dto = new()
                {
                    TaskSubmissionId = r.TaskSubmissionId,
                    MentorId = r.MentorId,
                    Feedback = r.Feedback,
                    Score = r.Score,
                    Status = r.Status,
                    ReviewedDate = r.ReviewedDate,
                };
                return dto;

            }
            catch (Exception ex)
            {
                throw new Exception("Error while converting review to DTO.", ex);
            }

        }
    }
}