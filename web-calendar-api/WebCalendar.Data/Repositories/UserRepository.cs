using System.Collections.Generic;
using System.Linq;
using WebCalendar.Data.Entities;
using WebCalendar.Data.Repositories.Interfaces;

namespace WebCalendar.Data.Repositories
{
  public class UserRepository : IUserRepository
  {
    private readonly IWebCalendarDbContext _context;

    public UserRepository(IWebCalendarDbContext context)
    {
      _context = context;
    }

    public IEnumerable<User> Get() => _context.Users.ToList();
    
    public void Create(User user)
    {
      _context.Users.Add(user);
      _context.SaveChanges();
    }
  }
}
