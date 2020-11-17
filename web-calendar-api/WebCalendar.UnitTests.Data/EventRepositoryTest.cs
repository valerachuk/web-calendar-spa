using System;
using Microsoft.EntityFrameworkCore;
using WebCalendar.Data;
using WebCalendar.Data.Entities;
using WebCalendar.Data.Repositories;
using Xunit;

namespace WebCalendar.UnitTests.Data
{
  public class EventRepositoryTest : IDisposable
  {
    private readonly WebCalendarDbContext _context;

    public EventRepositoryTest()
    {
      var options = new DbContextOptionsBuilder();
      options.UseInMemoryDatabase("WebCalendarEventRepositoryTestInMemoryDatabase");

      _context = new WebCalendarDbContext(options.Options);
    }

    public void Dispose()
    {
      _context.Database.EnsureDeleted();
      _context.Dispose();
    }

    [Fact]
    public void GetEventNotificationInfo_GettingEventInfo_ShouldReturnFullEventInfo()
    {
      // Arrange
      var user = new User
      {
        FirstName = "myUser1",
        ReceiveEmailNotifications = true,
        Email = "myUserEmail1@email.com"
      };

      var calendar = new Calendar
      {
        Name = "myCalendar1",
        User = user
      };

      var @event = new Event
      {
        Name = "myEvent1_1492132193",
        StartDateTime = new DateTime(2020, 5, 6),
        Reiteration = null,
        Calendar = calendar,
        Id = 3
      };

      _context.Events.Add(@event);
      _context.SaveChanges();

      var eventRepository = new EventRepository(_context);

      // Act
      var expectedEventNotificationInfo = eventRepository.GetEventNotificationInfo(3);

      // Assert
      Assert.NotNull(expectedEventNotificationInfo);
      Assert.Equal(expectedEventNotificationInfo.UserFirstName, user.FirstName);
      Assert.Equal(expectedEventNotificationInfo.UserWantsReceiveEmailNotifications, user.ReceiveEmailNotifications);
      Assert.Equal(expectedEventNotificationInfo.UserEmail, user.Email);
      Assert.Equal(expectedEventNotificationInfo.CalendarName, calendar.Name);
      Assert.Equal(expectedEventNotificationInfo.EventName, @event.Name);
      Assert.Equal(expectedEventNotificationInfo.StartDateTime, @event.StartDateTime);
      Assert.False(expectedEventNotificationInfo.IsSeries);
    }

    [Fact]
    public void GetEventNotificationInfo_GettingInvalidEventInfo_ShouldReturnNull()
    {
      // Arrange
      var eventRepository = new EventRepository(_context);

      // Act
      var expectedEventNotificationInfo = eventRepository.GetEventNotificationInfo(125134);

      // Assert
      Assert.Null(expectedEventNotificationInfo);
    }

  }
}
