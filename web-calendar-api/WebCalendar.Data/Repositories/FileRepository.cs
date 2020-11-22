using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebCalendar.Data.Entities;
using WebCalendar.Data.Repositories.Interfaces;

namespace WebCalendar.Data.Repositories
{
  public class FileRepository : IFileRepository
  {
    private readonly IWebCalendarDbContext _context;

    public FileRepository(IWebCalendarDbContext context)
    {
      _context = context;
    }

    public UserFile GetFile(int id) => _context.EventFile.Find(id);

    public UserFile GetEventFile(int eventId) => 
      _context.EventFile.AsNoTracking().Where(ef => ef.Event.Id == eventId).FirstOrDefault();

    public bool DeleteFile(UserFile file)
    {
      _context.EventFile.Remove(file);
      return _context.SaveChanges() > 0;
    }

    public int AddFile(UserFile file)
    {
      _context.EventFile.Add(file);
      _context.SaveChanges();
      return file.Id;
    }
  }
}
