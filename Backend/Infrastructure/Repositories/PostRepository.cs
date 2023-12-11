using Entity;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;


namespace Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    private readonly InstaLiteContext _context;
    

    public PostRepository(InstaLiteContext context)
    {
        _context = context;
        
    }

    public async Task<Post> AddPost(Post post)
    {

        post.CreatedAt = DateTime.Now;
        post.UpdatedAt = DateTime.Now;

        if (post.UserId == 0)
        {
            post.UserId = 1;
        }

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        return post;
    }

    public async Task<Post> DeletePostById(int id)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post == null)
            return null;

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();

        return post;
    }

    public async Task<Post> GetPost(int id)
    {
        return await _context.Posts.FindAsync(id);
    }

    public Task<List<Post>> GetPosts()
    {
        return _context.Posts.ToListAsync();
    }

    
}