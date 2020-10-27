using System.Collections.Generic;
using System.Linq;
using WebCalendar.Data.Entities;
using WebCalendar.Data.Repositories.Interfaces;

namespace WebCalendar.Data.Repositories
{
  public class CalendarRepository : ICalendarRepository
  {
    private readonly IWebCalendarDbContext _context;

    public CalendarRepository(IWebCalendarDbContext context)
    {
      _context = context;
    }

    public IEnumerable<Calendar> GetUserCalendars(int userId)
    {
      return _context.Calendars.Where(calendar => calendar.UserId == userId).ToList();
    }

    public int AddCalendar(Calendar calendar)
    {
      _context.Calendars.Add(calendar);
      _context.SaveChanges();
      return calendar.Id;
    }

    public bool DeleteCalendar(int id)
		{
      _context.Calendars.Remove(_context.Calendars.Find(id));
      return _context.SaveChanges() > 0;
		}
  }
}
