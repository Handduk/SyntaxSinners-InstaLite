using Entity;

namespace Infrastructure.Repositories
{
    public interface ICommentRepository
    {
        Task<Comment> AddComment(Comment comment);
        Task<Comment> GetComment(int id);
        Task<List<Comment>> GetComments();
 
        
    }
}
