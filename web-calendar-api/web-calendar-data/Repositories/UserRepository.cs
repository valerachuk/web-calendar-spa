using System.Collections.Generic;
using System.Linq;
using web_calendar_data.Entities;

namespace web_calendar_data.Repositories
{
  public interface IUserRepository
  {
    IEnumerable<User> Get();
  }

  public class UserRepository : IUserRepository
  {
    private readonly IWebCalendarDbContext _context;

    public UserRepository(IWebCalendarDbContext context)
    {
      _context = context;
    }

    public IEnumerable<User> Get() => _context.Users.ToList();
  }
}
