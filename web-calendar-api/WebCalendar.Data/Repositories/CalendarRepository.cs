using Microsoft.EntityFrameworkCore;
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

    public Calendar GetCalendar(int id) => _context.Calendars.AsNoTracking().FirstOrDefault(x => x.Id == id);

    public Calendar GetDefaultCalendar()
    {
      var defaultCalendars = _context.Calendars.AsNoTracking().Where(c => c.Name == "Default");
      int mainDefaultCalendarId = defaultCalendars.Min(c => c.Id);
      var defaultCalendar = _context.Calendars.Find(mainDefaultCalendarId);
      return defaultCalendar;
    }

    public Calendar GetCalendarWithEvents(int id)
      => _context.Calendars.Include(c => c.Events).FirstOrDefault(c => c.Id == id);

    public int AddCalendar(Calendar calendar)
    {
      _context.Calendars.Add(calendar);
      _context.SaveChanges();
      return calendar.Id;
    }

    public bool DeleteCalendar(int id)
    {
      var calendar = _context.Calendars.Find(id);
      if (calendar == null)
        return false;
      _context.Calendars.Remove(calendar);
      return _context.SaveChanges() > 0;
    }

    public bool EditCalendar(Calendar calendar)
    {
      var oldCalendar = _context.Calendars.Find(calendar.Id);
      if (oldCalendar == null)
        return false;
      oldCalendar.Name = calendar.Name;
      oldCalendar.Description = calendar.Description;
      return _context.SaveChanges() > 0;
    }
  }
}
