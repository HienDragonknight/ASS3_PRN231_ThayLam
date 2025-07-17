using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Repositories;
using Repositories.DTOs;
using BusinessObjects.Entities;

namespace ProjectManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize] // Require authentication for all controller actions
    public class OrchidController : ControllerBase
    {
        private readonly OrchidRepository _repository;

        public OrchidController(OrchidRepository repository)
        {
            _repository = repository;
        }

        // GET: api/orchid
        [HttpGet]
        [AllowAnonymous] // Allow unauthenticated access to view all products
        public async Task<IActionResult> GetAll()
        {
            var orchids = await _repository.GetAllOrchidsAsync();
            return Ok(orchids);
        }

        // GET: api/orchid/5
        [HttpGet("{id}")]
        [AllowAnonymous] // Allow unauthenticated access to view product details
        public async Task<IActionResult> Get(int id)
        {
            var orchid = await _repository.GetOrchidAsync(id);
            if (orchid == null) return NotFound();
            return Ok(orchid);
        }

        // POST: api/orchid
        [HttpPost]
        [AllowAnonymous] // Allow unauthenticated access to view all products
        public async Task<IActionResult> Create([FromBody] OrchidCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _repository.CreateOrchidAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.OrchidId }, created);
        }

        // PUT: api/orchid/5
        [HttpPut("{id}")]
        [AllowAnonymous] // Allow unauthenticated access to view all products
        public async Task<IActionResult> Update(int id, [FromBody] OrchidUpdateDto dto)
        {
            if (!await _repository.OrchidExistsAsync(id))
                return NotFound();

            var updated = await _repository.UpdateOrchidAsync(id, dto);
            return Ok(updated);
        }

        // DELETE: api/orchid/5
        [HttpDelete("{id}")]
        [AllowAnonymous] // Allow unauthenticated access to view all products
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _repository.DeleteOrchidAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
