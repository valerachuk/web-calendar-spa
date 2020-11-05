namespace WebCalendar.Business.Domains.Interfaces
{
  public interface INotificationSenderDomain
  {
    void NotifyEventCreated(string eventName, int userId);
  }
}
