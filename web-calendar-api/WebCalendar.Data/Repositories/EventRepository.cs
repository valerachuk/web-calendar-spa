using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
      return _context.Events
        .Where(calendarEvent => calendarEvent.Id == id)
        .Join(_context.Calendars,
        ev => ev.CalendarId,
        cal => cal.Id,
        (ev, cal) => new { ev, cal.UserId })
        .Select(c => new Tuple<Event, int>(c.ev, c.UserId))
        .FirstOrDefault();
    }

    public Event GetMainEvent(int id)
    {
      var seriesEvent = _context.Events.Find(id);

      // find min event id in series for getting main event of the series
      var mainSeriesEvent = _context.Events.Where(ev => ev.SeriesId == seriesEvent.SeriesId);
      int mainEventId = mainSeriesEvent.Min(ev => ev.Id);
      return _context.Events.Find(mainEventId);
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

    public Event UpdateCalendarEvent(Event calendarEvent)
    {
      Event currentEvent = _context.Events.Find(calendarEvent.Id);
      currentEvent.Calendar = calendarEvent.Calendar;
      currentEvent.CalendarId = calendarEvent.CalendarId;
      currentEvent.EndDateTime = calendarEvent.EndDateTime;
      currentEvent.Name = calendarEvent.Name;
      currentEvent.Venue = calendarEvent.Venue;
      currentEvent.NotificationTime = calendarEvent.NotificationTime;
      currentEvent.Reiteration = calendarEvent.Reiteration;
      currentEvent.StartDateTime = calendarEvent.StartDateTime;
      _context.SaveChanges();
      return currentEvent;
    }

    public void UpdateCalendarEventSeries(Event calendarEvent)
    {
      var currentEvent = _context.Events.Find(calendarEvent.Id);
      var currentEventSeries = _context.Events.Where(ev => ev.SeriesId == currentEvent.SeriesId);
      foreach (var item in currentEventSeries)
      {
        item.Calendar = calendarEvent.Calendar;
        item.CalendarId = calendarEvent.CalendarId;
        item.Name = calendarEvent.Name;
        item.Venue = calendarEvent.Venue;
        item.NotificationTime = calendarEvent.NotificationTime;
        item.StartDateTime = item.StartDateTime.Date
          + new TimeSpan(calendarEvent.StartDateTime.Hour, calendarEvent.StartDateTime.Minute, 0);
        item.EndDateTime = item.EndDateTime.Date
          + new TimeSpan(calendarEvent.EndDateTime.Hour, calendarEvent.EndDateTime.Minute, 0);
      }
      _context.SaveChanges();
    }
  }
}
