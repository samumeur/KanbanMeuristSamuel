using Microsoft.EntityFrameworkCore;
namespace Kanban.Models
{
    public class DBContext: DbContext
    {
        public DBContext(DbContextOptions<DBContext> options)
               : base(options)
        {
        }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Board> Board { get; set; }
    }
}