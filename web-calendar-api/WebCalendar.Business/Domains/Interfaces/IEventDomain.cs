using WebCalendar.Business.ViewModels;

namespace WebCalendar.Business.Domains.Interfaces
{
  public interface IEventDomain
  {
    EventViewModel GetEvent(int id);
    void AddCalendarEvent(EventViewModel calendarEvent);
    void UpdateCalendarEvent(EventViewModel calendarEvent, int userId);
    void UpdateCalendarEventSeries(EventViewModel calendarItem, int userId);
    void DeleteCalendarEvent(int id, int userId);
    void DeleteCalendarEventSeries(int id, int userId);
  }
}
