using System.ComponentModel.DataAnnotations;
using WebCalendar.Data.Entities;

namespace WebCalendar.Business.ViewModels
{
	public class CalendarViewModel
	{
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public int UserId { get; set; }
		public string Description { get; set; }
	}
}
