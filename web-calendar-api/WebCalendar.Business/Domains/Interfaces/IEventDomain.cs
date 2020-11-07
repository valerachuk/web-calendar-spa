using WebCalendar.Business.ViewModels;

namespace WebCalendar.Business.Domains.Interfaces
{
  public interface IEventDomain
  {
    EventViewModel GetEvent(int id);
    void AddCalendarEvent(EventViewModel calendarEvent);
    void DeleteCalendarEvent(int id, int userId);
    void DeleteCalendarEventSeries(int id, int userId);
  }
}
