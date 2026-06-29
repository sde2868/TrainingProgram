using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using TraineeManagement.Data;

namespace TrainingDirectory.Api.Controllers
{
    [ApiController]
    [Route("api/trainees")]
    public class TraineesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TraineesController> _logger;

        public TraineesController(AppDbContext context, ILogger<TraineesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("{id}/processing-profile")]
        public async Task<ActionResult<TraineeProcessingProfileResponse>>
            GetProcessingProfile(int id)
        {
            _logger.LogInformation("Processing profile requested for trainee {TraineeId}", id);

            var trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.id == id);
            if (trainee is null)
            {
                return NotFound();
            }
            // await Task.Delay(10000); // for time out resilience
            return Ok(
                new TraineeProcessingProfileResponse
                {
                    TraineeId = trainee.id,
                    FullName = $"{trainee.FirstName} {trainee.LastName}",
                    Email = trainee.Email,
                    TechStack = trainee.TechStack,
                    // temporary values
                    TotalAssignments = 0,
                    CompletedAssignments = 0
                });
        }
    }
}