using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Models;
using TraineeManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace TraineeManagement.Controllers
{
    [ApiController]
    [Route("api/processing-jobs")]
    [Authorize]
    public class ProcessingJobsController : ControllerBase
    {
        private readonly IProcessingJobService _service;

        public ProcessingJobsController(
            IProcessingJobService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            ProcessingJobResponseDTO? job = await _service.GetByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            return Ok(job);
        }
    }
}