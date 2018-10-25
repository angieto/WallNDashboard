using Microsoft.EntityFrameworkCore;

namespace TheWall.Models
{
    public class WallContext: DbContext
    {
        // base(options) calls the parent class's constructor
        public WallContext(DbContextOptions<WallContext> options) : base(options) {}
        public DbSet<User> Users {get; set;}
        public DbSet<Message> Messages {get; set;}
        public DbSet<Comment> Comments {get; set;}
    }
}