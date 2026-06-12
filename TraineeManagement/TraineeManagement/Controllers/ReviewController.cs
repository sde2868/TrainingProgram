using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Models;
using TraineeManagement.Services;
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
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            return Ok(await it.GetAll(search));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            Review? review = await it.GetById(id);
            if (review == null)
            {
                return NotFound(new { message = "Review not found!" });
            }
            return Ok(it.ReturnDTO(review));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] ReviewDTO dto)
        {
            Review review = await it.Create(dto);
            return Ok(review);
        }
    }
}