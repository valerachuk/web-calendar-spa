using WebCalendar.Data.Entities;

namespace WebCalendar.Business.Domains.Interfaces
{
  public interface INotificationSenderDomain
  {
    void ScheduleEventCreatedNotification(int eventId);
    void ScheduleEventEditedNotification(int eventId);
    
    void ScheduleEventSeriesStartedNotification(int seriesId);
    void ScheduleEventStartedNotification(int eventId);

    void NotifyEventDeleted(int eventId, bool isSeries);
    void CancelScheduledNotification(params Event[] events);
  }
}
