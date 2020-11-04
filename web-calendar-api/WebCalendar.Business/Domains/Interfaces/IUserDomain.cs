using WebCalendar.Business.ViewModels;

namespace WebCalendar.Business.Domains.Interfaces
{
  public interface IUserDomain
  {
    int? Authenticate(LoginViewModel login);
    int Register(RegisterViewModel login);
    bool HasUser(string email);
    string GenerateJWT(UserViewModel userId);
    UserViewModel GetUser(int id);
    bool EditUser(UserViewModel userView);
  }
}
