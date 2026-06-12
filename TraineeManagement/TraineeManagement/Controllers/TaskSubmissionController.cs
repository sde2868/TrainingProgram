using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Models;
using TraineeManagement.Services;
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
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            return Ok(await it.GetAll(search));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            TaskSubmission? taskSubmission = await it.GetById(id);
            if (taskSubmission == null)
            {
                return NotFound(new { message = "TaskSubmission not found!" });
            }
            return Ok(it.ReturnDTO(taskSubmission));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] TaskSubmissionDTO dto)
        {
            TaskSubmission taskSubmission = await it.Create(dto);
            return Ok(taskSubmission);
        }
    }
}
