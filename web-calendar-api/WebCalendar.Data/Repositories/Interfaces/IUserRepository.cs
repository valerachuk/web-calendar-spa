using System.Collections.Generic;
using WebCalendar.Data.Entities;

namespace WebCalendar.Data.Repositories.Interfaces
{
  public interface IUserRepository
  {
    IEnumerable<User> Get();
  }
}
