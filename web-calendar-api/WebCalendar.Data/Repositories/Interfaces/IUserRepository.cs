using System.Collections.Generic;
using WebCalendar.Data.Entities;

namespace WebCalendar.Data.Repositories.Interfaces
{
  public interface IUserRepository
  {
    User GetByEmail(string email);
    void Create(User user);
  }
}
