using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserIdentityService.Models;

namespace UserIdentityService.Data
{
  public class UserContext : IdentityDbContext<User>
  {

    public UserContext(DbContextOptions<UserContext> options) : base(options)
    {
      
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
