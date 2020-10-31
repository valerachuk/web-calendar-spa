using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WebCalendar.Data.Entities
{
  public class Event
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public int CalendarId { get; set; }
    public Calendar Calendar { get; set; }
    public DateTimeOffset StartDateTime { get; set; }
    public DateTimeOffset EndDateTime { get; set; }
    public string Venue { get; set; }
    public NotificationTime? NotificationTime { get; set; } // Minutes
    public Reiteration? Reiteration { get; set; }
    public int SeriesId { get; set; }
  }

  public enum NotificationTime
  {
    In10Minutes = 10,
    In15Minutes = 15,
    In30Minutes = 30,
    In1Hour = 60
  }

  public enum Reiteration
  {
    Daily = 1,
    Weekly = 7
  }
}
