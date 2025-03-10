using Microsoft.AspNetCore.Mvc;
using RodeFortune.DAL.Models;
using RodeFortune.DAL.Repositories;

namespace RodeFortune.BLL.Controllers
{
    [Route("api/natal-charts")]
    [ApiController]
    public class NatalChartController : ControllerBase
    {
        private readonly NatalChartRepository _natalChartRepository;

        public NatalChartController(NatalChartRepository natalChartRepository)
        {
            _natalChartRepository = natalChartRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var natalCharts = await _natalChartRepository.GetAllAsync();
            return Ok(natalCharts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var natalChart = await _natalChartRepository.GetByIdAsync(id);
            if (natalChart == null)
                return NotFound();
            return Ok(natalChart);
        }

        [HttpPost]
        public async Task<IActionResult> Create(NatalChart natalChart)
        {
            await _natalChartRepository.CreateAsync(natalChart);
            return CreatedAtAction(nameof(GetById), new { id = natalChart.Id }, natalChart);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, NatalChart natalChart)
        {
            var existingNatalChart = await _natalChartRepository.GetByIdAsync(id);
            if (existingNatalChart == null)
                return NotFound();

            await _natalChartRepository.UpdateAsync(id, natalChart);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existingNatalChart = await _natalChartRepository.GetByIdAsync(id);
            if (existingNatalChart == null)
                return NotFound();

            await _natalChartRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
