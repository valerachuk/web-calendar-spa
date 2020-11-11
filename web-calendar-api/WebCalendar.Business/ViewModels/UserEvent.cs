using System;
using System.Collections.Generic;
using System.Text;
using WebCalendar.Constants.Enums;

namespace WebCalendar.Data.Entities
{
  public class UserEvent
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
    public int UserId { get; set; }
  }
}
