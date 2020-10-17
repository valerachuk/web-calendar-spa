using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using web_calendar_data.Entities;

namespace web_calendar_data
{
  public interface IWebCalendarDbContext
  {
    DbSet<User> Users { get; set; }
  }

  public class WebCalendarDbContext : DbContext, IWebCalendarDbContext
  {
    public WebCalendarDbContext(DbContextOptions contextOptions) : base(contextOptions)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder
        .Entity<User>()
        .HasIndex(u => u.Email)
        .IsUnique();

      modelBuilder
        .Entity<User>()
        .HasData(new[]
        {
          new User
          {
            Id = 1, FirstName = "FN1", LastName = "LN1", Email = "user1@mail.com",
            PasswordHash = new byte[] {1, 2, 3, 4, 5}, Salt = new byte[] { 1, 2 }
          },
          new User
          {
            Id = 2, FirstName = "FN2", LastName = "LN2", Email = "user2@mail.com",
            PasswordHash = new byte[] {1, 2, 3, 4, 5}, Salt = new byte[] { 1, 2 }
          },
          new User
          {
            Id = 3, FirstName = "FN3", LastName = "LN3", Email = "user3@mail.com",
            PasswordHash = new byte[] {1, 2, 3, 4, 5}, Salt = new byte[] { 1, 2 }
          }
        });
    }

    public DbSet<User> Users { get; set; }
  }
}
