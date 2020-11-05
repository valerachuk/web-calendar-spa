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
    public IEnumerable<Event> GetClaendarsEventsByTimeInterval(DateTime startDateTime, DateTime endDateTime, int[] calendarsId)
    {
      List<Event> calendarEvents = new List<Event>();
      var eventsList = _context.Events.Where(calendarEvent =>
        calendarEvent.StartDateTime >= startDateTime &&
        calendarEvent.EndDateTime <= endDateTime &&
        calendarsId.Contains(calendarEvent.CalendarId)
     );
      calendarEvents.AddRange(eventsList);
      return calendarEvents;
    }
  }
}
