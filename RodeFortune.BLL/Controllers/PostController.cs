using Microsoft.AspNetCore.Mvc;
using RodeFortune.DAL.Models;
using RodeFortune.DAL.Repositories;

namespace RodeFortune.BLL.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly PostRepository _postRepository;

        public PostController(PostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var posts = await _postRepository.GetAllAsync();
            return Ok(posts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null)
                return NotFound();
            return Ok(post);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Post post)
        {
            await _postRepository.CreateAsync(post);
            return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Post post)
        {
            var existingPost = await _postRepository.GetByIdAsync(id);
            if (existingPost == null)
                return NotFound();

            await _postRepository.UpdateAsync(id, post);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existingPost = await _postRepository.GetByIdAsync(id);
            if (existingPost == null)
                return NotFound();

            await _postRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
