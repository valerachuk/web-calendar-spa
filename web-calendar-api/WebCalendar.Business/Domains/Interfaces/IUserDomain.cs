using System.Collections.Generic;
using WebCalendar.Business.ViewModels;

namespace WebCalendar.Business.Domains.Interfaces
{
  public interface IUserDomain
  {
    IEnumerable<UserViewModel> Get();
    int? Authenticate(LoginViewModel login);
    int? Register(RegisterViewModel login);
    string GenerateJWT(int userId);
  }
}
