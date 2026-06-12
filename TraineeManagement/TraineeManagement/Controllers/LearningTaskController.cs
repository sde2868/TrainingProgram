using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Models;
using TraineeManagement.Services;
using Microsoft.AspNetCore.Authorization;

namespace TraineeManagement.Controllers
{
    [ApiController]
    [Route("api/learning-tasks")]
    public class LearningTaskController(ILearningTask itlt) : ControllerBase
    {
        private readonly ILearningTask it = itlt;

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
            LearningTask? learningTask = await it.GetById(id);
            if (learningTask == null)
            {
                return NotFound(new { message = "Learning task not found!" });
            }
            return Ok(it.ReturnDTO(learningTask));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] LearningTaskDTO dto)
        {
            LearningTask learningTask = await it.Create(dto);
            return Ok(learningTask);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, [FromBody] LearningTaskDTO dto)
        {
            LearningTask? learningTask = await it.Put(id, dto);
            return Ok(it.ReturnDTO(learningTask));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteById(int id)
        {
            LearningTask? learningTask = await it.DeleteById(id);
            return NoContent();
        }
    }
}