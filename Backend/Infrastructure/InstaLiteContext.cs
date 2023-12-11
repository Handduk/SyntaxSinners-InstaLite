using Entity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class InstaLiteContext : DbContext
    {
        public InstaLiteContext(DbContextOptions<InstaLiteContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<Comment> Comments { get; set; }

    }
}
