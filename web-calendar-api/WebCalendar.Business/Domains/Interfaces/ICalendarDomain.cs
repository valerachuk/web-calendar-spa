using System.Collections.Generic;
using WebCalendar.Business.DTO;
using WebCalendar.Business.ViewModels;

namespace WebCalendar.Business.Domains.Interfaces
{
  public interface ICalendarDomain
  {
    IEnumerable<CalendarViewModel> GetUserCalendars(int id);
    CalendarViewModel GetCalendar(int id);
    int AddCalendar(CalendarViewModel calendar, int userId);
    bool DeleteCalendar(int id, int userId);
    bool EditCalendar(CalendarViewModel calendarView, int userId);
    CalendarICSDTO CreateICS(int calendarId, int userId);
  }
}
