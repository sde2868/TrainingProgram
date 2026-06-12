using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Models;
using TraineeManagement.Services;
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
        public async Task<IActionResult> GetTrainees([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] TraineeStatus? status = null)
        {
            var result = await it.GetTraineesAsync( pageNumber, pageSize, search, status);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            Trainee? trainee = await it.GetById(id);
            if (trainee == null)
            {
                return NotFound(new { message = "Trainee not found!" });
            }
            return Ok(it.ReturnDTO(trainee));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] TraineeDTO dto)
        {
            Trainee trainee = await it.Create(dto);
            return Ok(trainee);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, [FromBody] TraineeDTO dto)
        {
            Trainee? trainee = await it.Put(id, dto);
            return Ok(it.ReturnDTO(trainee));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteById(int id)
        {
            Trainee? trainee = await it.DeleteById(id);
            return NoContent();
        }
    }
}
