using Entity;

namespace Infrastructure.Repositories
{
    public interface IPostRepository
    {
        Task<Post> AddPost(Post post);
        Task<Post> DeletePostById(int id);
        Task<Post> GetPost(int id);
        Task<List<Post>> GetPosts();
        
    }
}
