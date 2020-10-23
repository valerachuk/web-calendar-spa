using Microsoft.EntityFrameworkCore;
using WebCalendar.Data.Entities;

namespace WebCalendar.Data
{
  public class WebCalendarDbContext : DbContext, IWebCalendarDbContext
  {
    public WebCalendarDbContext(DbContextOptions contextOptions) : base(contextOptions)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      var userModelBuilder = modelBuilder.Entity<User>();

      userModelBuilder
        .HasIndex(u => u.Email)
        .IsUnique();

      userModelBuilder
        .Property(user => user.Email)
        .IsRequired();

      userModelBuilder
        .Property(user => user.FirstName)
        .IsRequired();

      userModelBuilder
        .Property(user => user.LastName)
        .IsRequired();

      userModelBuilder
        .Property(user => user.PasswordHash)
        .IsRequired();
      
      userModelBuilder
        .Property(user => user.Salt)
        .IsRequired();

      userModelBuilder
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

      var calendarModelBuilder = modelBuilder.Entity<Calendar>();

      calendarModelBuilder
        .Property(calendar => calendar.Name)
        .IsRequired()
        .HasMaxLength(100);

      calendarModelBuilder
        .Property(calendar => calendar.Description)
        .HasMaxLength(1000);

      calendarModelBuilder
        .HasOne(calendar => calendar.User)
        .WithMany(user => user.Calendars)
        .HasForeignKey(calendar => calendar.UserId)
        .IsRequired();
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Calendar> Calendars { get; set; }
  }
}
