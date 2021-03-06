﻿using System.Collections.Generic;
using WebCalendar.Data.Entities;

namespace WebCalendar.Data.Repositories.Interfaces
{
  public interface ICalendarRepository
  {
    IEnumerable<Calendar> GetUserCalendars(int userId);
    Calendar GetCalendar(int id);
    Calendar GetDefaultCalendar(int userId);
    Calendar GetCalendarWithEvents(int id);
    int AddCalendar(Calendar calendar);
    bool DeleteCalendar(int id);
    bool EditCalendar(Calendar calendar);
  }
}
