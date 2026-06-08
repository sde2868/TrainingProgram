using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Models;
using TraineeManagement.Services;

namespace TraineeManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TraineeController : ControllerBase
    {
        private readonly ITrainee it;
        public TraineeController(ITrainee itr)
        {
            it = itr;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            return Ok(await it.GetAll(search));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var trainee = await it.GetById(id);
            if (trainee == null)
            {
                return NotFound(new { message = "Trainee not found!" });
            }
            return Ok(it.ReturnDTO(trainee));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TraineeDTO dto)
        {
            var trainee = await it.Create(dto);
            return Ok(trainee);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] TraineeDTO dto)
        {
            var trainee = await it.Put(id, dto);
            return Ok(it.ReturnDTO(trainee));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            var trainee = await it.DeleteById(id);
            return NoContent();
        }
    }
}
