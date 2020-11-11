using System;
using System.Collections.Generic;
using WebCalendar.Data.DTO;
using WebCalendar.Data.Entities;

namespace WebCalendar.Data.Repositories.Interfaces
{
  public interface IEventRepository
  {
    Tuple<Event, int> GetEvent(int id);
    Event GetMainEvent(int id);
    IEnumerable<Event> GetSeries(int seriesId);
    void AddSeriesOfCalendarEvents(IEnumerable<Event> calendarEvent, int? seriesId);
    Event AddCalendarEvents(Event calendarEvent);
    Event UpdateCalendarEvent(Event calendarEvent);
    void UpdateCalendarEventSeries(Event calendarEvent);
    void UpdateEvent(Event calendarEvent);
    Event DeleteCalendarEvent(int calendarEventId);
    IEnumerable<Event> DeleteCalendarEventSeries(int calendarEventId);
    EventNotificationDTO GetEventNotificationInfo(int id);
  }
}
