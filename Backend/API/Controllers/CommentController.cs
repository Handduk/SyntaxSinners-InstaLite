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
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CommentController> _logger;

        public CommentController(
            ICommentRepository commentRepository, 
            IMapper mapper,
            ILogger<CommentController> logger)
        {
            _commentRepository = commentRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<CommentDto>>> GetPosts()
        {
            try
            {
                var comments = await _commentRepository.GetComments();
                var commentsDto = _mapper.Map<List<CommentDto>>(comments);
                return Ok(commentsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get comments.");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<CommentDto>> GetComment(int id)
        {
            var comment = await _commentRepository.GetComment(id);
            if (comment == null)
            {
                return NotFound();
            }
            var commentDto = _mapper.Map<CommentDto>(comment);
            return Ok(commentDto);
        }

        [HttpPost]
        public IActionResult CreateComment([FromBody] CommentDto commentDto)
        {
            try
            {
                Comment comment = _mapper.Map<Comment>(commentDto);

                _commentRepository.AddComment(comment);

                return Ok("Comment created successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


    }
}
