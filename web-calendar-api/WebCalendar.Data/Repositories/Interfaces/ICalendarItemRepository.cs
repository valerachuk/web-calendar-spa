using System;
using System.Collections.Generic;
using WebCalendar.Data.Entities;

namespace WebCalendar.Data.Repositories.Interfaces
{
  public interface ICalendarItemRepository
  {
    IEnumerable<Event> GetCalendarsEventsByTimeInterval(DateTime startDateTime, DateTime endDateTime, int[] calendarsId);
    IEnumerable<Event> GetSharedCalendarEventsByTimeInterval(DateTime startDateTime, DateTime endDateTime, int[] calendarsId, int userId);
    void UpdateCalendarsEventTime(DateTime startDateTime, DateTime endDateTime, int id);
  }
}
