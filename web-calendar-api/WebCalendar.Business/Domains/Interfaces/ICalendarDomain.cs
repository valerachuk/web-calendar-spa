using System.Collections.Generic;
using WebCalendar.Business.ViewModels;

namespace WebCalendar.Business.Domains.Interfaces
{
  public interface ICalendarDomain
  {
    IEnumerable<CalendarViewModel> GetUserCalendars(int id);
    int AddCalendar(CalendarViewModel calendar);
    bool DeleteCalendar(int id);
  }
}
