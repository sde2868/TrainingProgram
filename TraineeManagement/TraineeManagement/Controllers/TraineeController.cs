using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Models;
using TraineeManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace TraineeManagement.Controllers
{
    [ApiController]
    [Route("api/trainees")]
    public class TraineeController(ITrainee itr) : ControllerBase
    {
        private readonly ITrainee it = itr;

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllTrainees([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] TraineeStatus? status = null)
        {
            var result = await it.GetAllTrainees(pageNumber, pageSize, search, status);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetTraineeById(int id)
        {
            Trainee? trainee = await it.GetTraineeById(id);
            if (trainee == null)
            {
                return NotFound(new { message = "Trainee not found!" });
            }
            return Ok(it.ReturnTraineeDTO(trainee));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateTrainee([FromBody] TraineeDTO dto)
        {
            Trainee trainee = await it.CreateTrainee(dto);
            return Ok(trainee);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateTrainee(int id, [FromBody] TraineeDTO dto)
        {
            Trainee? trainee = await it.UpdateTrainee(id, dto);
            return Ok(it.ReturnTraineeDTO(trainee));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTraineeById(int id)
        {
            Trainee? trainee = await it.DeleteTraineeById(id);
            return NoContent();
        }
    }
}
