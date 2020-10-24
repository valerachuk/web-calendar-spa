using WebCalendar.Business.ViewModels;

namespace WebCalendar.Business.Domains.Interfaces
{
  public interface IUserDomain
  {
    int? Authenticate(LoginViewModel login);
    int Register(RegisterViewModel login);
    bool HasUser(string email);
    string GenerateJWT(int userId);
    UserViewModel GetUser(int id);
  }
}
