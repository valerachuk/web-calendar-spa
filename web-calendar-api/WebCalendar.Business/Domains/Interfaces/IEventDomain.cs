using System.Collections.Generic;
using WebCalendar.Business.ViewModels;

namespace WebCalendar.Business.Domains.Interfaces
{
  public interface IEventDomain
  {
    EventViewModel GetEvent(int id);
    void AddCalendarEvent(EventViewModel calendarEvent);
    void DeleteCalendarEvent(int id, IEnumerable<int> calendarsId);
    void DeleteCalendarEventSeries(int id, IEnumerable<int> calendarsId);
  }
}
