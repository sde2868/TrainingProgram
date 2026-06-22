using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Models;
using TraineeManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace TraineeManagement.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    public class ReviewController(IReview ir) : ControllerBase
    {
        private readonly IReview it = ir;

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllReviews([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] ReviewStatus? status = null)
        {
            return Ok(await it.GetAllReviews(pageNumber, pageSize, search, status));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetReviewById(int id)
        {
            Review? review = await it.GetReviewById(id);
            if (review == null)
            {
                return NotFound(new { message = "Review not found!" });
            }
            return Ok(it.ReturnReviewDTO(review));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateReview([FromBody] ReviewDTO dto)
        {
            Review review = await it.CreateReview(dto);
            return Ok(review);
        }
    }
}