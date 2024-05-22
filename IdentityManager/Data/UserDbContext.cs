using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityManager.Data
{
  public class UserDbContext : IdentityDbContext
  {

    public UserDbContext(DbContextOptions options) : base(options)
    {

    }


  }
}
