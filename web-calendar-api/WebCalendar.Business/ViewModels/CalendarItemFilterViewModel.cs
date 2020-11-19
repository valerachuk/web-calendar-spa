using System;
using System.ComponentModel.DataAnnotations;

namespace WebCalendar.Business.ViewModels
{
  public class CalendarItemFilterViewModel
  {
    [Required]
    public DateTime Start { get; set; }
    [Required]
    public DateTime End { get; set; }
    [Required]
    public int[] Id { get; set; }
  }
}
