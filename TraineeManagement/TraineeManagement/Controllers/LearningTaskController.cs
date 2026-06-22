using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Models;
using TraineeManagement.Interfaces;
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
        public async Task<IActionResult> GetAllLearningTasks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] LearningTaskStatus? status = null)
        {
            return Ok(await it.GetAllLearningTasks(pageNumber, pageSize, search, status));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetLearningTaskById(int id)
        {
            LearningTask? learningTask = await it.GetLearningTaskById(id);
            if (learningTask == null)
            {
                return NotFound(new { message = "Learning task not found!" });
            }
            return Ok(it.ReturnLearningTaskDTO(learningTask));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateLearningTask([FromBody] LearningTaskDTO dto)
        {
            LearningTask learningTask = await it.CreateLearningTask(dto);
            return Ok(learningTask);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateLearningTaskById(int id, [FromBody] LearningTaskDTO dto)
        {
            LearningTask? learningTask = await it.UpdateLearningTaskById(id, dto);
            return Ok(it.ReturnLearningTaskDTO(learningTask));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteLearningById(int id)
        {
            LearningTask? learningTask = await it.DeleteLearningById(id);
            return NoContent();
        }
    }
}