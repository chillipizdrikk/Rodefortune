using Microsoft.AspNetCore.Mvc;
using RodeFortune.DAL.Models;
using RodeFortune.DAL.Repositories;

namespace RodeFortune.BLL.Controllers
{
    [Route("api/readings")]
    [ApiController]
    public class ReadingController : ControllerBase
    {
        private readonly ReadingRepository _readingRepository;

        public ReadingController(ReadingRepository readingRepository)
        {
            _readingRepository = readingRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var readings = await _readingRepository.GetAllAsync();
            return Ok(readings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var reading = await _readingRepository.GetByIdAsync(id);
            if (reading == null)
                return NotFound();
            return Ok(reading);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Reading reading)
        {
            await _readingRepository.CreateAsync(reading);
            return CreatedAtAction(nameof(GetById), new { id = reading.Id }, reading);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Reading reading)
        {
            var existingReading = await _readingRepository.GetByIdAsync(id);
            if (existingReading == null)
                return NotFound();

            await _readingRepository.UpdateAsync(id, reading);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existingReading = await _readingRepository.GetByIdAsync(id);
            if (existingReading == null)
                return NotFound();

            await _readingRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
