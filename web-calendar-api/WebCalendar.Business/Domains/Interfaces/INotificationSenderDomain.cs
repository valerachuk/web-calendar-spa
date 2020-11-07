namespace WebCalendar.Business.Domains.Interfaces
{
  public interface INotificationSenderDomain
  {
    void ScheduleEventCreatedNotification(int eventId);
    void ScheduleEventStartedNotification(int eventId);
  }
}
