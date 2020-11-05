using System;
using System.ComponentModel.DataAnnotations;

namespace WebCalendar.Business.ViewModels
{
  public class CalendarItemViewModel
  {
    public int Id { get; set; }
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
    [Required]
    public DateTime StartDateTime { get; set; }
    [Required]
    public DateTime EndDateTime { get; set; }
  }
}
