using Microsoft.EntityFrameworkCore;
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
        calendarEvent.EndDateTime >= startDateTime &&
        calendarEvent.StartDateTime <= endDateTime &&
        calendarsId
        .Contains(calendarEvent.CalendarId))
        .ToList();
      return eventsList;
    }

    public IEnumerable<Event> GetSharedCalendarEventsByTimeInterval(DateTime startDateTime, DateTime endDateTime, int[] calendarId, int userId)
    {
      var defaultCalendarId = _context.Calendars.Where(cal => cal.UserId == userId).Min(cal => cal.Id);
      if (defaultCalendarId > 0 && calendarId.Contains(defaultCalendarId)) {
        var sharedEventsList = _context.Events
          .Include(ev => ev.Guests)
          .ThenInclude(eventGuests => eventGuests.User)
          .Where(calendarEvent =>
            calendarEvent.EndDateTime >= startDateTime &&
            calendarEvent.StartDateTime <= endDateTime &&
            calendarEvent.Guests
            .Any(x => x.UserId == userId && x.EventId == calendarEvent.Id))
          .ToArray();
        return sharedEventsList;
      }
      return new Event[] { };
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
