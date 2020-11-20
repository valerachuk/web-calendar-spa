using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebCalendar.Data.Entities;
using WebCalendar.Data.Repositories.Interfaces;

namespace WebCalendar.Data.Repositories
{
  public class EventFileRepository : IEventFileRepository
  {
    private readonly IWebCalendarDbContext _context;

    public EventFileRepository(IWebCalendarDbContext context)
    {
      _context = context;
    }

    public EventFile GetFile(int id) => _context.EventFile.Find(id);

    public EventFile GetEventFile(int eventId) => 
      _context.EventFile.Where(ef => ef.EventId == eventId).FirstOrDefault();

    public int AddEventFile(EventFile eventFile)
    {
      _context.EventFile.Add(eventFile);
      _context.SaveChanges();
      return eventFile.Id;
    }
  }
}
