using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebCalendar.Data.Entities;
using WebCalendar.Data.Repositories.Interfaces;

namespace WebCalendar.Data.Repositories
{
  public class EventRepository : IEventRepository
  {
    private readonly IWebCalendarDbContext _context;
    public EventRepository(IWebCalendarDbContext context)
    {
      _context = context;
    }
    public void AddSeriesOfCalendarEvents(Event calendarEvent, int seriesId)
    {
      _context.Events.Add(calendarEvent);
      _context.SaveChanges();
    }
    public int AddCalendarEvents(Event calendarEvent)
    {
      _context.Events.Add(calendarEvent);
      _context.SaveChanges();
      return calendarEvent.SeriesId;
    }
  }
}
