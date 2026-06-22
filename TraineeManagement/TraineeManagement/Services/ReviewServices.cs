using TraineeManagement.Models;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Data;
using TraineeManagement.Interfaces;

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
                _logger.LogError(ex, "Error while seeding review data.");
                throw new Exception($"Error while seeding review data.", ex);
            }
        }

        public async Task<PaginationDTO<ReviewDTO>> GetAllReviews(int pageNumber = 1, int pageSize = 10, string? search = null, ReviewStatus? status = null)
        {
            try
            {   
                _logger.LogInformation($"GetAllReviews: searching for {search} in all reviews.");
                var query = _context.Reviews.AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();

                    query = query.Where(r =>
                        r.Feedback.ToLower().Contains(search)
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
                var reviews = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(t => new ReviewDTO
                    {
                        // id = t.id,
                        Feedback = t.Feedback,
                        Score = t.Score,
                        TaskSubmissionId = t.TaskSubmissionId,
                        MentorId = t.MentorId,
                        Status = t.Status,
                        ReviewedDate = t.ReviewedDate
                    })
                    .ToListAsync();


                _logger.LogInformation($"GetAllReviews: reviews ({totalRecords}) fetched successfully for search {search}.");
                return new PaginationDTO<ReviewDTO>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalRecords,
                    Data = reviews
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllReviews: error fetching reviews.");
                throw new Exception($"Error while deleting reviews.", ex);
            }
        }

        public async Task<Review?> GetReviewById(int id)
        {
            try
            {
                _logger.LogInformation($"GetReviewById: fetching review by id: {id}.");
                Review? review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
                if (review == null)
                {
                    _logger.LogWarning($"GetReviewById: review with id {id} not found!");
                }
                return review;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetReviewById: error while fetching review with id {id}.");
                throw new Exception($"Error while feetching review with id {id}.", ex);
            }

        }

        public async Task<Review> CreateReview(ReviewDTO dto)
        {
            try
            {
                _logger.LogInformation($"CreateReview: creating review.");
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

                _logger.LogInformation($"CreateReview: review created successfully. Id: {review.Id}");
                return review;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateReview: error while creating review.");
                throw new Exception($"Error while creating review.", ex);
            }

        }

        public ReviewDTO ReturnReviewDTO(Review r)
        {
            try
            {
                _logger.LogInformation($"ReturnReviewDTO: converting review with id {r.Id} to DTO.");
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
                _logger.LogError($"ReturnReviewDTO: error converting review with id {r.Id} to DTO.");
                throw new Exception("Error while converting review to DTO.", ex);
            }

        }
    }
}