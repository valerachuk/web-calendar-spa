using System;
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
    public Tuple<Event, int> GetEvent(int id)
    {
        return _context.Events.Where(calendarEvent => calendarEvent.Id == id)
        .Join(_context.Calendars,
        ev => ev.CalendarId,
        cal => cal.Id,
        (ev, cal) => new { ev, cal.UserId }).Select(c => new Tuple<Event, int>(c.ev, c.UserId)).FirstOrDefault();
    }

    public void AddSeriesOfCalendarEvents(IEnumerable<Event> calendarEvents, int? seriesId)
    {
      _context.Events.AddRange(calendarEvents);
      _context.SaveChanges();
    }
    public int? AddCalendarEvents(Event calendarEvent)
    {
      _context.Events.Add(calendarEvent);
      _context.SaveChanges();

      if (calendarEvent.Reiteration == null)
      {
        return null;
      }
      return calendarEvent.SeriesId;
    }

    public void DeleteCalendarEvent(int calendarEventId)
    {
      var currentEvent = _context.Events.Find(calendarEventId);
      _context.Events.Remove(currentEvent);
      _context.SaveChanges();
    }

    public void DeleteCalendarEventSeries(int calendarEventId)
    {
      Event currentEvent = _context.Events.Find(calendarEventId);
      IEnumerable<Event> eventSeries = _context.Events.Where(ev => ev.SeriesId == currentEvent.SeriesId);
      _context.Events.RemoveRange(eventSeries);
      _context.SaveChanges();
    }
  }
}
