using Microsoft.AspNetCore.Http;
using WebCalendar.Business.ViewModels;

namespace WebCalendar.Business.Domains.Interfaces
{
  public interface IEventFileDomain
  {
    EventFileViewModel GetEventFile(int eventId);
    void DeleteEventFile(int eventId);
    int AddFile(IFormFile file, int eventId);
  }
}
