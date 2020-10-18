using System.Collections.Generic;
using WebCalendar.Business.ViewModels;

namespace WebCalendar.Business.Domains.Interfaces
{
  public interface IUserDomain
  {
    IEnumerable<UserViewModel> Get();
  }
}
