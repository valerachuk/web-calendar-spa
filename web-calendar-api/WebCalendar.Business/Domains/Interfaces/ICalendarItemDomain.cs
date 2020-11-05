using System;
using System.Collections.Generic;
using WebCalendar.Business.ViewModels;

namespace WebCalendar.Business.Domains.Interfaces
{
  public interface ICalendarItemDomain
  {
    IEnumerable<CalendarItemViewModel> GetCalendarsItemsByTimeInterval(DateTime startDateTime, DateTime endDateTme, int[] calendarsId);
  }
}
