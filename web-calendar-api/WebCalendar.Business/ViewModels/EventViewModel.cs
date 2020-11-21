using System;
using System.ComponentModel.DataAnnotations;
using WebCalendar.Constants.Enums;

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
    public NotificationTime? NotificationTime { get; set; }
    public Reiteration? Reiteration { get; set; }
    public int? FileId { get; set; }
  }
}
