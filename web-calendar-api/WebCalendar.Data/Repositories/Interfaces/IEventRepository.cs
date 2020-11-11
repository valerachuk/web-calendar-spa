using System;
using System.Collections.Generic;
using WebCalendar.Data.Entities;

namespace WebCalendar.Data.Repositories.Interfaces
{
  public interface IEventRepository
  {
    Tuple<Event, int> GetEvent(int id);
    Event GetMainEvent(int id);
    void AddSeriesOfCalendarEvents(IEnumerable<Event> calendarEvent, int? seriesId);
    int? AddCalendarEvents(Event calendarEvent);
    Event UpdateCalendarEvent(Event calendarEvent);
    void UpdateCalendarEventSeries(Event calendarEvent);
    void DeleteCalendarEvent(int calendarEventId);
    void DeleteCalendarEventSeries(int calendarEventId);
  }
}
