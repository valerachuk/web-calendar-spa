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
    public Event GetEvent(int id)
    {
      return _context.Events.Where(calendarEvent => calendarEvent.Id == id).FirstOrDefault();
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

      // If it has no reiteration, seriesId must to be null 
      if (calendarEvent.Reiteration == null)
      {
        return null;
      }
      return calendarEvent.SeriesId;
    }

    public void DeleteCalendarEvent(int calendarEventId, IEnumerable<int> calendarsId)
    {
      var currentEvent = _context.Events.Find(calendarEventId);
      _context.Events.Remove(currentEvent);
      _context.SaveChanges();
    }

    public void DeleteCalendarEventSeries(int calendarEventId, IEnumerable<int> calendarsId)
    {
      Event currentEvent = _context.Events.Find(calendarEventId);
      IEnumerable<Event> eventSeries = _context.Events.Where(ev => ev.SeriesId == currentEvent.SeriesId);
      _context.Events.RemoveRange(eventSeries);
      _context.SaveChanges();
    }
  }
}
