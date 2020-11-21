using WebCalendar.Data.Entities;

namespace WebCalendar.Data.Repositories.Interfaces
{
  public interface IFileRepository
  {
    UserFile GetFile(int id);
    UserFile GetEventFile(int eventId);
    bool DeleteFile(UserFile file);
    int AddFile(UserFile file);
  }
}
