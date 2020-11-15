using System;
using System.Linq;
using WebCalendar.Data;
using WebCalendar.Data.Entities;
using WebCalendar.Data.Repositories;
using Xunit;

namespace WebCalendar.TestData
{
  public class EventRepositoryTest : IClassFixture<WebCalendarInMemoryDbContextFixture>
  {
    private readonly WebCalendarDbContext _context;

    public EventRepositoryTest(WebCalendarInMemoryDbContextFixture dbFixture)
    {
      _context = dbFixture.Context;
    }

    [Fact]
    public void GetEventNotificationInfo_GettingEventInfoShouldReturnFullEventInfo()
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
        Calendar = calendar
      };

      _context.Events.Add(@event);
      _context.SaveChanges();

      var eventRepository = new EventRepository(_context);

      // Act
      var expectedEventNotificationInfo =
        eventRepository.GetEventNotificationInfo(_context.Events.First(evt => evt.Name == @event.Name).Id);

      // Assert
      Assert.NotNull(expectedEventNotificationInfo);
      Assert.Equal(expectedEventNotificationInfo.UserFirstName, user.FirstName);
      Assert.Equal(expectedEventNotificationInfo.UserWantsReceiveEmailNotifications, user.ReceiveEmailNotifications);
      Assert.Equal(expectedEventNotificationInfo.UserEmail, user.Email);
      Assert.Equal(expectedEventNotificationInfo.CalendarName, calendar.Name);
      Assert.Equal(expectedEventNotificationInfo.EventName, @event.Name);
      Assert.Equal(expectedEventNotificationInfo.StartDateTime, @event.StartDateTime);
      Assert.Equal(expectedEventNotificationInfo.IsSeries, @event.Reiteration != null);

    }

    [Fact]
    public void GetEventNotificationInfo_GettingInvalidEventInfoReturnsNull()
    {
      // Arrange
      var eventRepository = new EventRepository(_context);
      var invalidIndex = _context.Events.Select(evt => evt.Id).DefaultIfEmpty().Max() + 1;

      // Act
      var expectedEventNotificationInfo = eventRepository.GetEventNotificationInfo(invalidIndex);

      // Assert
      Assert.Null(expectedEventNotificationInfo);
    }

  }
}
