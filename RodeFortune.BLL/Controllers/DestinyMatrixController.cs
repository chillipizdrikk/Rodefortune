using Microsoft.AspNetCore.Mvc;
using RodeFortune.DAL.Models;
using RodeFortune.DAL.Repositories;

namespace RodeFortune.BLL.Controllers
{
    [Route("api/destiny-matrices")]
    [ApiController]
    public class DestinyMatrixController : ControllerBase
    {
        private readonly DestinyMatrixRepository _destinyMatrixRepository;

        public DestinyMatrixController(DestinyMatrixRepository destinyMatrixRepository)
        {
            _destinyMatrixRepository = destinyMatrixRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var destinyMatrices = await _destinyMatrixRepository.GetAllAsync();
            return Ok(destinyMatrices);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var destinyMatrix = await _destinyMatrixRepository.GetByIdAsync(id);
            if (destinyMatrix == null)
                return NotFound();
            return Ok(destinyMatrix);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DestinyMatrix destinyMatrix)
        {
            await _destinyMatrixRepository.CreateAsync(destinyMatrix);
            return CreatedAtAction(nameof(GetById), new { id = destinyMatrix.Id }, destinyMatrix);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, DestinyMatrix destinyMatrix)
        {
            var existingDestinyMatrix = await _destinyMatrixRepository.GetByIdAsync(id);
            if (existingDestinyMatrix == null)
                return NotFound();

            await _destinyMatrixRepository.UpdateAsync(id, destinyMatrix);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existingDestinyMatrix = await _destinyMatrixRepository.GetByIdAsync(id);
            if (existingDestinyMatrix == null)
                return NotFound();

            await _destinyMatrixRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
