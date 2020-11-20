using WebCalendar.Data.Entities;

namespace WebCalendar.Data.Repositories.Interfaces
{
  public interface IEventFileRepository
  {
    EventFile GetFile(int id);
    EventFile GetEventFile(int eventId);
    int AddEventFile(EventFile eventFile);
  }
}
