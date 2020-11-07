using System;
using System.Collections.Generic;
using WebCalendar.Data.Entities;

namespace WebCalendar.Data.Repositories.Interfaces
{
  public interface IEventRepository
  {
    Event GetEvent(int id);
    void AddSeriesOfCalendarEvents(IEnumerable<Event> calendarEvent, int seriesId);
    Event AddCalendarEvents(Event calendarEvent);
    void UpdateEvent(Event calendarEvent);
  }
}
