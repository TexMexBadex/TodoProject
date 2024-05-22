using Microsoft.EntityFrameworkCore;

namespace TaskMicroservice.Data
{
  public class TaskDbContext : DbContext
  {
    public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options)
    {
    }

    public DbSet<TaskItem> Tasks { get; set; }
  }

 
}
