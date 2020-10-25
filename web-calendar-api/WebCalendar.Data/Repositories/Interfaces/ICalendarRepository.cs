using System.Collections.Generic;
using WebCalendar.Data.Entities;

namespace WebCalendar.Data.Repositories.Interfaces
{
  public interface ICalendarRepository
  {
    IEnumerable<Calendar> GetUserCalendars(int userId);
    int AddCalendar(Calendar calendar);
  }
}
