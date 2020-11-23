using System;
using System.Collections.Generic;
using WebCalendar.Data.Entities;

namespace WebCalendar.Data.DTO
{
  public class EventNotificationDTO
  {
    public bool UserWantsReceiveEmailNotifications { get; set; }
    public string EventName { get; set; }
    public string CalendarName { get; set; }
    public string UserEmail { get; set; }
    public string UserFirstName { get; set; }
    public DateTime StartDateTime { get; set; }
    public bool IsSeries { get; set; }
    public List<EventGuests> Guests { get; set; }
  }
}
