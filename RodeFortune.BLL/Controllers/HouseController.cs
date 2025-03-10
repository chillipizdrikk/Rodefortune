using Microsoft.AspNetCore.Mvc;
using RodeFortune.DAL.Models;
using RodeFortune.DAL.Repositories;

namespace RodeFortune.BLL.Controllers
{
    [Route("api/houses")]
    [ApiController]
    public class HouseController : ControllerBase
    {
        private readonly HouseRepository _houseRepository;

        public HouseController(HouseRepository houseRepository)
        {
            _houseRepository = houseRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var houses = await _houseRepository.GetAllAsync();
            return Ok(houses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var house = await _houseRepository.GetByIdAsync(id);
            if (house == null)
                return NotFound();
            return Ok(house);
        }

        [HttpPost]
        public async Task<IActionResult> Create(House house)
        {
            await _houseRepository.CreateAsync(house);
            return CreatedAtAction(nameof(GetById), new { id = house.Id }, house);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, House house)
        {
            var existingHouse = await _houseRepository.GetByIdAsync(id);
            if (existingHouse == null)
                return NotFound();

            await _houseRepository.UpdateAsync(id, house);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existingHouse = await _houseRepository.GetByIdAsync(id);
            if (existingHouse == null)
                return NotFound();

            await _houseRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
