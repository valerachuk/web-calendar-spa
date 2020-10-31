using System;
using System.Collections.Generic;
using System.Text;
using WebCalendar.Data.Entities;

namespace WebCalendar.Data.Repositories.Interfaces
{
  public interface IEventRepository
  {
    void AddSeriesOfCalendarEvents(Event calendarEvent, int seriesId);
    int AddCalendarEvents(Event calendarEvent); // return event series id
  }
}
