﻿using System.Linq;
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

    public User GetByEmail(string email) => _context.Users.FirstOrDefault(user => user.Email == email);

    public User GetUser(int id) => _context.Users.Find(id);
    
    public void Create(User user)
    {
      _context.Users.Add(user);
      _context.SaveChanges();
      Calendar defaultCalendar = new Calendar() { Name = "Default calendar", UserId = user.Id };
      _context.Calendars.Add(defaultCalendar);
      _context.SaveChanges();
    }
  }
}
