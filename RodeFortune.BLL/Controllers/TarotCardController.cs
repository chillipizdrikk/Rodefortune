using Microsoft.AspNetCore.Mvc;
using RodeFortune.DAL.Models;
using RodeFortune.DAL.Repositories;

namespace RodeFortune.BLL.Controllers
{
    [Route("api/tarot-cards")]
    [ApiController]
    public class TarotCardController : ControllerBase
    {
        private readonly TarotCardRepository _tarotCardRepository;

        public TarotCardController(TarotCardRepository tarotCardRepository)
        {
            _tarotCardRepository = tarotCardRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tarotCards = await _tarotCardRepository.GetAllAsync();
            return Ok(tarotCards);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var tarotCard = await _tarotCardRepository.GetByIdAsync(id);
            if (tarotCard == null)
                return NotFound();
            return Ok(tarotCard);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TarotCard tarotCard)
        {
            await _tarotCardRepository.CreateAsync(tarotCard);
            return CreatedAtAction(nameof(GetById), new { id = tarotCard.Id }, tarotCard);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, TarotCard tarotCard)
        {
            var existingTarotCard = await _tarotCardRepository.GetByIdAsync(id);
            if (existingTarotCard == null)
                return NotFound();

            await _tarotCardRepository.UpdateAsync(id, tarotCard);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existingTarotCard = await _tarotCardRepository.GetByIdAsync(id);
            if (existingTarotCard == null)
                return NotFound();

            await _tarotCardRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
