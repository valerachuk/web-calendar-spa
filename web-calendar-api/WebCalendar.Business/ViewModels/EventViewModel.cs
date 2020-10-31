using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WebCalendar.Business.ViewModels
{
  public class EventViewModel
  {
    public int Id { get; set; }
    [Required]
    [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters.")]
    public string Name { get; set; }
    [Required]
    public int CalendarId { get; set; }
    [Required]
    public DateTime StartDateTime { get; set; }
    [Required]
    public DateTime EndDateTime { get; set; }
    [StringLength(100, ErrorMessage = "Venue can't be longer than 100 characters.")]
    public string Venue { get; set; }
    public NotificationTimeViewModel? NotificationTime { get; set; }
    public ReiterationViewModel? Reiteration { get; set; }
  }

  public enum NotificationTimeViewModel
  {
    In10Minutes = 10,
    In15Minutes = 15,
    In30Minutes = 30,
    In1Hour = 60
  }

  public enum ReiterationViewModel
  {
    Daily = 1,
    Weekly = 7
  }
}
