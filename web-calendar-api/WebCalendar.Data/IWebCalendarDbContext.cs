using Microsoft.EntityFrameworkCore;
using WebCalendar.Data.Entities;

namespace WebCalendar.Data
{
  public interface IWebCalendarDbContext
  {
    DbSet<User> Users { get; set; }
    DbSet<Calendar> Calendars { get; set; }
    DbSet<Event> Events { get; set; }

    int SaveChanges();
  }
}
