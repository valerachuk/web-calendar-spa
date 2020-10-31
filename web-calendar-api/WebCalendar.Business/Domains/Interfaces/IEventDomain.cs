using System;
using WebCalendar.Business.ViewModels;

namespace WebCalendar.Business.Domains.Interfaces
{
  public interface IEventDomain
  {
    void AddCalendarEvent(EventViewModel calendarEvent);
  }
}
