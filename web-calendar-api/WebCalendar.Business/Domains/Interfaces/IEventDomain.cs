using System;
using System.Collections.Generic;
using System.Text;
using WebCalendar.Business.ViewModels;

namespace WebCalendar.Business.Domains.Interfaces
{
  public interface IEventDomain
  {
    void AddCalendarEvent(EventViewModel calendarEvent);
  }
}
