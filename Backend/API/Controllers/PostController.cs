using Api.Dtos;
using Entity;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PostController> _logger;

        public PostController(
            IPostRepository postRepository, 
            IMapper mapper,
            ILogger<PostController> logger)
        {
            _postRepository = postRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<PostDto>>> GetPosts()
        {
            try
            {
                var posts = await _postRepository.GetPosts();
                var postsDto = _mapper.Map<List<PostDto>>(posts);
                return Ok(postsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get posts.");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<PostDto>> GetPost(int id)
        {
            var post = await _postRepository.GetPost(id);
            if (post == null)
            {
                _logger.LogError("Post not found.");
                return NotFound();
            }
            var postDto = _mapper.Map<PostDto>(post);
            return Ok(postDto);
        }

        [HttpPost]
        public async Task<ActionResult<PostDto>> CreatePost([FromForm] PostWithImageDto postDto)
        {
            try
            {
                var post = await HandleImageUpload(postDto);

                _mapper.Map(postDto, post);

                await _postRepository.AddPost(post);

                var createdPostDto = _mapper.Map<PostDto>(post);

                return CreatedAtAction(nameof(GetPost), new { id = createdPostDto.Id }, createdPostDto);
            }
            catch (Exception ex)
            {
                
                _logger.LogError(ex, "Failed to create post.");
                return BadRequest(new { error = "Failed to create post." });
            }
        }

        private async Task<Post> HandleImageUpload(PostWithImageDto postDto)
        {
            if (postDto.ImageFile == null || postDto.ImageFile.Length == 0)
            {
                return null;
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(postDto.ImageFile.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new ArgumentException("Invalid file format. Supported formats: jpg, jpeg, png.");
            }

            var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
            var imagePath = Path.Combine("wwwroot/media", uniqueFileName);

            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await postDto.ImageFile.CopyToAsync(stream);
            }

            return new Post { Image = uniqueFileName };
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<PostDto>> DeletePost(int id)
        {
            try
            {
                var post = await _postRepository.DeletePostById(id);
                if (post == null)
                {
                    _logger.LogError("Post not found.");
                    return NotFound();
                }
                var postDto = _mapper.Map<PostDto>(post);
                _logger.LogInformation("Post succesfully deleted.");
                return Ok(postDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete post.");
                return BadRequest(new { error = "Failed to delete post." });
            }
        }

    }
}
