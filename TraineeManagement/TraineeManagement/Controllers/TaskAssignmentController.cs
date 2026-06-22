using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Models;
using TraineeManagement.Interfaces;
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
        public async Task<IActionResult> GetAllTaskAssignments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] TaskAssignmentStatus? status = null)
        {
            return Ok(await it.GetAllTaskAssignments(pageNumber, pageSize, search, status));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetTaskAssignmentById(int id)
        {
            TaskAssignment? taskAssignment = await it.GetTaskAssignmentById(id);
            if (taskAssignment == null)
            {
                return NotFound(new { message = "TaskAssignment not found!" });
            }
            return Ok(it.ReturnTaskAssignmentDTO(taskAssignment));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateTaskAssignment([FromBody] TaskAssignmentDTO dto)
        {
            TaskAssignment taskAssignment = await it.CreateTaskAssignment(dto);
            return Ok(taskAssignment);
        }

        [HttpPut("{id}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateTaskAssignmentStatus(int id, [FromBody] TaskAssignmentStatus status)
        {
            TaskAssignment? taskAssignment = await it.UpdateTaskAssignmentStatus(id, status);
            return Ok(it.ReturnTaskAssignmentDTO(taskAssignment));
        }
    }
}
