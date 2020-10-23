using Microsoft.EntityFrameworkCore;
using WebCalendar.Data.Entities;

namespace WebCalendar.Data
{
  public interface IWebCalendarDbContext
  {
    DbSet<User> Users { get; set; }

    int SaveChanges();
  }
}
