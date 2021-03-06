﻿using WebCalendar.Business.DTO;
using WebCalendar.Business.ViewModels;

namespace WebCalendar.Business.Domains.Interfaces
{
  public interface IEventDomain
  {
    EventViewModel GetEvent(int id);
    int AddCalendarEvent(EventViewModel calendarEvent, bool isUpdated = false);
    void UpdateCalendarEvent(EventViewModel calendarEvent, int userId);
    void UpdateCalendarEventSeries(EventViewModel calendarItem, int userId);
    void DeleteCalendarEvent(int id, int userId);
    void DeleteCalendarEventSeries(int id, int userId);
    void UnsubscribeSharedEvent(int id, int guestId);
    void UnsubscribeSharedEventSeries(int id, int guestId);
    CalendarICSDTO CreateEventICS(int eventId, int userId);
    CalendarICSDTO CreateEventSeriesICS(int eventId, int userId);
  }
}
