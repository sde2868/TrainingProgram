using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Models;
using TraineeManagement.Services;

namespace TraineeManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TraineeController : ControllerBase
    {
        private readonly ITrainee it;
        public TraineeController(ITrainee itr)
        {
            it = itr;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(it.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var trainee = it.GetById(id);
            if(trainee == null)
            {
                return NotFound(new { message = "Trainee not found!"});
            }
            return Ok(it.ReturnDTO(trainee));
        }

        [HttpPost]
        public IActionResult Create([FromBody] TraineeDTO dto)
        {
            var trainee = it.Create(dto);
            return Ok(trainee);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] TraineeDTO dto)
        {
            var trainee = it.Put(id, dto);
            return Ok(it.ReturnDTO(trainee));
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteById(int id)
        {
            var trainee = it.DeleteById(id);
            return NoContent();
        }
    }
}
