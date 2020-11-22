using System;
using System.Collections.Generic;
using WebCalendar.Constants.Enums;

namespace WebCalendar.Data.Entities
{
  public class Event
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public int CalendarId { get; set; }
    public Calendar Calendar { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string Venue { get; set; }
    public NotificationTime? NotificationTime { get; set; } // Minutes
    public Reiteration? Reiteration { get; set; }
    public int? SeriesId { get; set; }
    public string NotificationScheduleJobId { get; set; }
    public int? FileId { get; set; }
    public UserFile File { get; set; }
    public List<EventGuests> Guests { get; set; }

    public Event()
    {
      Guests = new List<EventGuests>();
    }
  }
}
