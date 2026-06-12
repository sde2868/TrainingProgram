using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Models;
using TraineeManagement.Services;
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
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            return Ok(await it.GetAll(search));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            Mentor? mentor = await it.GetById(id);
            if (mentor == null)
            {
                return NotFound(new { message = "Mentor not found!" });
            }
            return Ok(it.ReturnDTO(mentor));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] MentorDTO dto)
        {
            Mentor mentor = await it.Create(dto);
            return Ok(mentor);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, [FromBody] MentorDTO dto)
        {
            Mentor? mentor = await it.Put(id, dto);
            return Ok(it.ReturnDTO(mentor));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteById(int id)
        {
            Mentor? mentor = await it.DeleteById(id);
            return NoContent();
        }
    }
}
