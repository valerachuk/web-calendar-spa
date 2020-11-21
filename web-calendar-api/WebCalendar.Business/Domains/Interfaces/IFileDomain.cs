using Microsoft.AspNetCore.Http;
using WebCalendar.Business.ViewModels;
using WebCalendar.Data.Entities;

namespace WebCalendar.Business.Domains.Interfaces
{
  public interface IFileDomain
  {
    FileViewModel GetEventFile(int eventId);
    void DeleteFile(int fileId);
    int AddFile(IFormFile file);
  }
}
