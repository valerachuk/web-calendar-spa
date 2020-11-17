using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebCalendar.Data;
using WebCalendar.Data.Entities;
using WebCalendar.Data.Repositories;
using Xunit;

namespace WebCalendar.UnitTests.Data
{
  public class UserRepositoryTest : IDisposable
  {
    private readonly WebCalendarDbContext _context;

    public UserRepositoryTest()
    {
      var options = new DbContextOptionsBuilder();
      options.UseInMemoryDatabase("WebCalendarUserRepositoryTestInMemoryDatabase");

      _context = new WebCalendarDbContext(options.Options);
    }

    public void Dispose()
    {
      _context.Database.EnsureDeleted();
      _context.Dispose();
    }

    [Fact]
    public void GetByEmail_GettingNonExistentUser_ShouldReturnNull()
    {
      // Arrange
      var user1 = new User
      {
        Email = "email1"
      };

      var user2 = new User
      {
        Email = "email2"
      };

      _context.Users.AddRange(user1, user2);
      _context.SaveChanges();

      var userRepository = new UserRepository(_context);

      // Act
      var actualUser = userRepository.GetByEmail("email3");

      // Assert
      Assert.Null(actualUser);

    }

    [Fact]
    public void GetByEmail_GettingUser_ShouldReturnUser()
    {
      // Arrange
      var user1 = new User
      {
        Email = "email1"
      };

      var user2 = new User
      {
        Email = "email2"
      };

      _context.Users.AddRange(user1, user2);
      _context.SaveChanges();

      var userRepository = new UserRepository(_context);

      // Act
      var actualUser = userRepository.GetByEmail("email2");

      // Assert
      Assert.Equal(user2.Id, actualUser.Id);

    }

    [Fact]
    public void GetUser_GettingNonExistentUser_ShouldReturnNull()
    {
      // Arrange
      var user1 = new User
      {
        Id = 2
      };

      var user2 = new User
      {
        Id = 4
      };

      _context.Users.AddRange(user1, user2);
      _context.SaveChanges();

      var userRepository = new UserRepository(_context);

      // Act
      var actualUser = userRepository.GetUser(3);

      // Assert
      Assert.Null(actualUser);

    }
    
    [Fact]
    public void GetUser_GettingUser_ShouldReturnUser()
    {
      // Arrange
      var user1 = new User
      {
        Id = 2
      };

      var user2 = new User
      {
        Id = 4
      };

      _context.Users.AddRange(user1, user2);
      _context.SaveChanges();

      var userRepository = new UserRepository(_context);

      // Act
      var actualUser = userRepository.GetUser(2);

      // Assert
      Assert.NotNull(actualUser);

    }

    [Fact]
    public void Create_CreatingUser_ShouldCreateUserWithDefaultCalendar()
    {
      // Arrange
      var user = new User
      {
        Id = 44
      };

      var userRepository = new UserRepository(_context);

      // Act
      userRepository.Create(user);

      // Assert
      var actualUser = _context.Users.First(usr => usr.Id == user.Id);
      Assert.Single(actualUser.Calendars);
    }

    [Fact]
    public void Edit_EditingUser_ShouldEditUser()
    {
      // Arrange
      const int id = 33;

      var initialUser = new User
      {
        Id = id,
        FirstName = "fn1",
        LastName = "ln1",
        ReceiveEmailNotifications = true
      };

      var editedUser = new User
      {
        Id = id,
        FirstName = "fn2",
        LastName = "ln2",
        ReceiveEmailNotifications = false
      };

      _context.Users.Add(initialUser);
      _context.SaveChanges();

      var userRepository = new UserRepository(_context);

      // Act
      var isEdited = userRepository.Edit(editedUser);

      // Assert
      var newUser = _context.Users.Find(id);

      Assert.True(isEdited);
      Assert.Equal(editedUser.FirstName, newUser.FirstName);
      Assert.Equal(editedUser.LastName, newUser.LastName);
      Assert.Equal(editedUser.ReceiveEmailNotifications, newUser.ReceiveEmailNotifications);

    }

    [Fact]
    public void Edit_EditingUnknownUser_ShouldReturnFalse()
    {
      // Arrange

      var user1 = new User
      {
        Id = 1,
        FirstName = "fn1",
        LastName = "ln1",
        ReceiveEmailNotifications = true
      };

      var user2 = new User
      {
        Id = 2,
        FirstName = "fn2",
        LastName = "ln2",
        ReceiveEmailNotifications = false
      };

      _context.Users.AddRange(user1, user2);
      _context.SaveChanges();

      var userRepository = new UserRepository(_context);

      // Act
      var isEdited = userRepository.Edit(new User
      {
        Id = 3
      });

      // Assert
      Assert.False(isEdited);

    }
    [Fact]

    public void Edit_EditingUserWithoutChanges_ShouldReturnFalse()
    {
      // Arrange
      const int id = 33;

      var initialUser = new User
      {
        Id = id,
        FirstName = "fn1",
        LastName = "ln1",
        ReceiveEmailNotifications = true
      };

      var editedUser = new User
      {
        Id = id,
        FirstName = "fn1",
        LastName = "ln1",
        ReceiveEmailNotifications = true
      };

      _context.Users.Add(initialUser);
      _context.SaveChanges();

      var userRepository = new UserRepository(_context);

      // Act
      var isEdited = userRepository.Edit(editedUser);

      // Assert
      var newUser = _context.Users.Find(id);

      Assert.False(isEdited);
      Assert.Equal(editedUser.FirstName, newUser.FirstName);
      Assert.Equal(editedUser.LastName, newUser.LastName);
      Assert.Equal(editedUser.ReceiveEmailNotifications, newUser.ReceiveEmailNotifications);

    }

  }
}
