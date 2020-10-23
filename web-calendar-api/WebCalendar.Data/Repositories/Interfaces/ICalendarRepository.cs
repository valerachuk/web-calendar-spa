using System.Collections.Generic;
using WebCalendar.Data.Entities;

namespace WebCalendar.Data.Repositories.Interfaces
{
	public interface ICalendarRepository
	{
		IEnumerable<Calendar> GetUserCalendars(int UserId);
		int AddCalendar(Calendar calendar);
	}
}
