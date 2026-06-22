using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Models;
using TraineeManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace TraineeManagement.Controllers
{
    [ApiController]
    [Route("api/submissions")]
    public class TaskSubmissionController(ITaskSubmission ita) : ControllerBase
    {
        private readonly ITaskSubmission it = ita;

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllTaskSubmissions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] TaskSubmissionStatus? status = null)
        {
            return Ok(await it.GetAllTaskSubmissions(pageNumber, pageSize, search, status));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetTaskSubmissionById(int id)
        {
            TaskSubmission? taskSubmission = await it.GetTaskSubmissionById(id);
            if (taskSubmission == null)
            {
                return NotFound(new { message = "TaskSubmission not found!" });
            }
            return Ok(it.ReturnTaskSubmissionDTO(taskSubmission));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateTaskSubmission([FromBody] TaskSubmissionDTO dto)
        {
            TaskSubmission taskSubmission = await it.CreateTaskSubmission(dto);
            return Ok(taskSubmission);
        }
    }
}
