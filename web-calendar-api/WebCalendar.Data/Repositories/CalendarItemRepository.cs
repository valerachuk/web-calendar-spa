using System;
using System.Collections.Generic;
using System.Linq;
using WebCalendar.Data.Entities;
using WebCalendar.Data.Repositories.Interfaces;

namespace WebCalendar.Data.Repositories
{
  public class CalendarItemRepository : ICalendarItemRepository
  {
    private readonly IWebCalendarDbContext _context;
    public CalendarItemRepository(IWebCalendarDbContext context)
    {
      _context = context;
    }
    public IEnumerable<Event> GetCalendarsEventsByTimeInterval(DateTime startDateTime, DateTime endDateTime, int[] calendarsId)
    {
      var eventsList = _context.Events
        .Where(calendarEvent =>
        calendarEvent.StartDateTime >= startDateTime &&
        calendarEvent.EndDateTime <= endDateTime &&
        calendarsId
        .Contains(calendarEvent.CalendarId))
        .ToList();
      return eventsList;
    }

    public void UpdateCalendarsEventTime(DateTime startDateTime, DateTime endDateTime, int id)
    {
      var currentEvent = _context.Events.Find(id);
      currentEvent.StartDateTime = startDateTime;
      currentEvent.EndDateTime = endDateTime;
      _context.SaveChanges();
    }
  }
}
