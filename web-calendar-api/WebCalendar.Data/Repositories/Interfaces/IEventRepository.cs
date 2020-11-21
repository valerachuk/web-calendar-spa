using System;
using System.Collections.Generic;
using WebCalendar.Data.DTO;
using WebCalendar.Data.Entities;

namespace WebCalendar.Data.Repositories.Interfaces
{
  public interface IEventRepository
  {
    Event GetEvent(int id);
    IEnumerable<Event> GetCalendarEvents(int calendarId);
    Event GetMainEvent(int id);
    IEnumerable<Event> GetSeries(int seriesId);
    UserEventDTO GetEventInfo(int id);
    void AddSeriesOfCalendarEvents(IEnumerable<Event> calendarEvent, int? seriesId);
    Event AddCalendarEvents(Event calendarEvent);
    Event UpdateCalendarEvent(Event calendarEvent);
    IEnumerable<Event> UpdateCalendarEventSeries(Event calendarEvent);
    void UpdateEvent(Event calendarEvent);
    Event DeleteCalendarEvent(int calendarEventId);
    IEnumerable<Event> DeleteCalendarEventSeries(int calendarEventId);
    void UnsubscribeSharedEvent(int id, int guestId);
    void UnsubscribeSharedEventSeries(int id, int guestId);
    EventNotificationDTO GetEventNotificationInfo(int id);
  }
}
