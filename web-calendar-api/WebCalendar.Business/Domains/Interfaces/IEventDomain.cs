using WebCalendar.Business.ViewModels;

namespace WebCalendar.Business.Domains.Interfaces
{
  public interface IEventDomain
  {
    EventViewModel GetEvent(int id);
    void AddCalendarEvent(EventViewModel calendarEvent);
  }
}
