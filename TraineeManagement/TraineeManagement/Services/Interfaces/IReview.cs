using TraineeManagement.Models;

namespace TraineeManagement.Interfaces
{
    public interface IReview
    {
        public Task<PaginationDTO<ReviewDTO>> GetAllReviews(int pageNumber = 1, int pageSize = 10, string? search = null, ReviewStatus? status = null);
        public Task<Review?> GetReviewById(int id);
        public Task<Review> CreateReview(ReviewDTO dto);
        ReviewDTO ReturnReviewDTO(Review review);
    }
}