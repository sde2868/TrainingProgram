using TraineeManagement.Models;

namespace TraineeManagement.Services
{
    public interface IReview
    {
        public Task<List<Review>> GetAll(string? search);
        public Task<Review?> GetById(int id);
        public Task<Review> Create(ReviewDTO dto);
        ReviewDTO ReturnDTO(Review review);
    }
}