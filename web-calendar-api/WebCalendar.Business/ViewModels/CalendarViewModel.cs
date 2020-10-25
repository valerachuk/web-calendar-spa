using System.ComponentModel.DataAnnotations;
using WebCalendar.Data.Entities;

namespace WebCalendar.Business.ViewModels
{
  public class CalendarViewModel
  {
    public int Id { get; set; }
    [Required]
    [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters.")]
    public string Name { get; set; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "User id must be greater then 0")]
    public int UserId { get; set; }
    [StringLength(1000, ErrorMessage = "Description can't be longer than 1000 characters.")]
    public string Description { get; set; }
  }
}
