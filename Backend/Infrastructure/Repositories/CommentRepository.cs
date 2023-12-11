using Entity;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;


namespace Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly InstaLiteContext _context;
    

    public CommentRepository(InstaLiteContext context)
    {
        _context = context;
        
    }

    public async Task<Comment> AddComment(Comment comment)
    {
        comment.CreatedAt = DateTime.Now;

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return comment;
    }

    public async Task<Comment> GetComment(int id)
    {
        return await _context.Comments.FindAsync(id);
    }

    public Task<List<Comment>> GetComments()
    {
        return _context.Comments.ToListAsync();
    }
}