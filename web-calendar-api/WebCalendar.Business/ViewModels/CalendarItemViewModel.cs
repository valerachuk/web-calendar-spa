using System;
using System.ComponentModel.DataAnnotations;
using WebCalendar.Constants.Enums;

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
    [Required]
    public CalendarItemType MetaType { get; set; }
    public Reiteration? Reiteration { get; set; }
  }
}
