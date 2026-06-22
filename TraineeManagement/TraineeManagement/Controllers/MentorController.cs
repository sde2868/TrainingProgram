using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Models;
using TraineeManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace TraineeManagement.Controllers
{
    [ApiController]
    [Route("api/mentors")]
    public class MentorController(IMentor itm) : ControllerBase
    {
        private readonly IMentor it = itm;

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllMentors([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] MentorStatus? status = null)
        {
            return Ok(await it.GetAllMentors(pageNumber, pageSize, search, status));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetMentorById(int id)
        {
            Mentor? mentor = await it.GetMentorById(id);
            if (mentor == null)
            {
                return NotFound(new { message = "Mentor not found!" });
            }
            return Ok(it.ReturnMentorDTO(mentor));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateMentor([FromBody] MentorDTO dto)
        {
            Mentor mentor = await it.CreateMentor(dto);
            return Ok(mentor);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateMentor(int id, [FromBody] MentorDTO dto)
        {
            Mentor? mentor = await it.UpdateMentor(id, dto);
            return Ok(it.ReturnMentorDTO(mentor));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteMentorById(int id)
        {
            Mentor? mentor = await it.DeleteMentorById(id);
            return NoContent();
        }
    }
}
