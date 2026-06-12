using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Models;
using TraineeManagement.Services;
using Microsoft.AspNetCore.Authorization;

namespace TraineeManagement.Controllers
{
    [ApiController]
    [Route("api/task-assignments")]
    public class TaskAssignmentController(ITaskAssignment ita) : ControllerBase
    {
        private readonly ITaskAssignment it = ita;

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
            TaskAssignment? taskAssignment = await it.GetById(id);
            if (taskAssignment == null)
            {
                return NotFound(new { message = "TaskAssignment not found!" });
            }
            return Ok(it.ReturnDTO(taskAssignment));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] TaskAssignmentDTO dto)
        {
            TaskAssignment taskAssignment = await it.Create(dto);
            return Ok(taskAssignment);
        }

        [HttpPut("{id}/status")]
        [Authorize]
        public async Task<IActionResult> Put(int id, [FromBody] TaskAssignmentStatus status)
        {
            TaskAssignment? taskAssignment = await it.Put(id, status);
            return Ok(it.ReturnDTO(taskAssignment));
        }
    }
}
