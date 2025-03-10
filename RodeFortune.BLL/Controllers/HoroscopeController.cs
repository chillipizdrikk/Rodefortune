using Microsoft.AspNetCore.Mvc;
using RodeFortune.DAL.Models;
using RodeFortune.DAL.Repositories;

namespace RodeFortune.BLL.Controllers
{
    [Route("api/horoscopes")]
    [ApiController]
    public class HoroscopeController : ControllerBase
    {
        private readonly HoroscopeRepository _horoscopeRepository;

        public HoroscopeController(HoroscopeRepository horoscopeRepository)
        {
            _horoscopeRepository = horoscopeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var horoscopes = await _horoscopeRepository.GetAllAsync();
            return Ok(horoscopes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var horoscope = await _horoscopeRepository.GetByIdAsync(id);
            if (horoscope == null)
                return NotFound();
            return Ok(horoscope);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Horoscope horoscope)
        {
            await _horoscopeRepository.CreateAsync(horoscope);
            return CreatedAtAction(nameof(GetById), new { id = horoscope.Id }, horoscope);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Horoscope horoscope)
        {
            var existingHoroscope = await _horoscopeRepository.GetByIdAsync(id);
            if (existingHoroscope == null)
                return NotFound();

            await _horoscopeRepository.UpdateAsync(id, horoscope);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existingHoroscope = await _horoscopeRepository.GetByIdAsync(id);
            if (existingHoroscope == null)
                return NotFound();

            await _horoscopeRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
