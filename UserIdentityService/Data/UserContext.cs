using Microsoft.EntityFrameworkCore;
using UserIdentityService.Models;

namespace UserIdentityService.Data
{
  public class UserContext : DbContext
  {

    public DbSet<User> Users { get; set; }

    protected readonly IConfiguration Configuration;


    public UserContext(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
      // connect to postgres with connection string from app settings
      options.UseNpgsql(Configuration.GetConnectionString("Database"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // Ensuring the ID is used as the primary key
      modelBuilder.Entity<User>()
        .HasKey(u => u.Id);

      modelBuilder.Entity<User>()
        .HasIndex(u => u.Email)
        .IsUnique();  // Ensures no two users can register with the same email

      modelBuilder.Entity<User>()
        .HasIndex(u => u.Username)
        .IsUnique();  // Ensures no two users can have the same username
        

      modelBuilder.Entity<User>()
        .Property(u => u.PasswordHash)
        .IsRequired();  // Makes PasswordHash a required field

      modelBuilder.Entity<User>()
        .Property(u => u.Name)
        .IsRequired();  // Makes name a required field

    }
  }
}
