using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingDirectory.Api.DTOs;
using TraineeManagement.Data;

namespace TrainingDirectory.Api.Controllers
{
    [ApiController]
    [Route("api/trainees")]
    public class TraineesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TraineesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}/processing-profile")]
        public async Task<ActionResult<TraineeProcessingProfileResponse>>
            GetProcessingProfile(int id)
        {
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