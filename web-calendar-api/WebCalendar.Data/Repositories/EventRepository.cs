using System.Collections.Generic;
using System.Linq;
using WebCalendar.Data.Entities;
using WebCalendar.Data.Repositories.Interfaces;

namespace WebCalendar.Data.Repositories
{
  public class EventRepository : IEventRepository
  {
    private readonly IWebCalendarDbContext _context;
    public EventRepository(IWebCalendarDbContext context)
    {
      _context = context;
    }
    public Event GetEvent(int id)
    {
      return _context.Events.Where(calendarEvent => calendarEvent.Id == id).FirstOrDefault();
    }

    public void AddSeriesOfCalendarEvents(IEnumerable<Event> calendarEvents, int seriesId)
    {
      _context.Events.AddRange(calendarEvents);
      _context.SaveChanges();
    }

    public Event AddCalendarEvents(Event calendarEvent)
    {
      _context.Events.Add(calendarEvent);
      _context.SaveChanges();
      return calendarEvent;
    }

    public void UpdateEvent(Event calendarEvent)
    {
      _context.Events.Update(calendarEvent);
      _context.SaveChanges();
    }
  }
}
