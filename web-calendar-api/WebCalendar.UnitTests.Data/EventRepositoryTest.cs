using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebCalendar.Data;
using WebCalendar.Data.DTO;
using WebCalendar.Data.Entities;
using WebCalendar.Data.Repositories;
using Xunit;

namespace WebCalendar.UnitTests.Data
{
  [Collection("Sequential")]
  public class EventRepositoryTest : IDisposable
  {
    private readonly WebCalendarDbContext _context;

    public EventRepositoryTest()
    {
      var options = new DbContextOptionsBuilder();
      options.UseInMemoryDatabase("WebCalendarTestInMemoryDatabase");

      _context = new WebCalendarDbContext(options.Options);

    }

    public void Dispose()
    {
      _context.Database.EnsureDeleted();
      _context.Dispose();
    }

    private Event GetTestEvent(int id)
    {
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
        Id = id
      };

      _context.Events.Add(@event);
      _context.SaveChanges();

      return @event;
    }

    [Fact]
    public void GetEventNotificationInfo_GettingEventInfo_ShouldReturnFullEventInfo()
    {
      // Arrange
      Event @event = GetTestEvent(3);

      var eventRepository = new EventRepository(_context);

      // Act
      var expectedEventNotificationInfo = eventRepository.GetEventNotificationInfo(3);

      // Assert
      Assert.NotNull(expectedEventNotificationInfo);
      Assert.Equal(expectedEventNotificationInfo.UserFirstName, @event.Calendar.User.FirstName);
      Assert.Equal(expectedEventNotificationInfo.UserWantsReceiveEmailNotifications, @event.Calendar.User.ReceiveEmailNotifications);
      Assert.Equal(expectedEventNotificationInfo.UserEmail, @event.Calendar.User.Email);
      Assert.Equal(expectedEventNotificationInfo.CalendarName, @event.Calendar.Name);
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

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    [InlineData(0)]
    public void GetEvent_GettingInvalidEvent_ShouldThrowException(int id)
    {
      // Arrange
      var eventRepository = new EventRepository(_context);

      // Act Assert
      Assert.Throws<ArgumentNullException>(()=> eventRepository.GetEvent(id));
    }

    [Fact]
    public void GetEvent_GettingValidEvent_ShouldReturnEvent()
    {
      // Arrange
      Event @event = GetTestEvent(3);

      var eventRepository = new EventRepository(_context);

      // Act
      var expectedEvent = eventRepository.GetEvent(3);

      // Assert
      Assert.Equal(expectedEvent.Calendar, @event.Calendar);
      Assert.Equal(expectedEvent.CalendarId, @event.CalendarId);
      Assert.Equal(expectedEvent.EndDateTime, @event.EndDateTime);
      Assert.Equal(expectedEvent.Id, @event.Id);
      Assert.Equal(expectedEvent.Name, @event.Name);
      Assert.Equal(expectedEvent.NotificationScheduleJobId, @event.NotificationScheduleJobId);
      Assert.Equal(expectedEvent.NotificationTime, @event.NotificationTime);
      Assert.Equal(expectedEvent.Reiteration, @event.Reiteration);
      Assert.Equal(expectedEvent.SeriesId, @event.SeriesId);
      Assert.Equal(expectedEvent.StartDateTime, @event.StartDateTime);
      Assert.Equal(expectedEvent.Venue, @event.Venue);
    }

    [Fact]
    public void GetEvent_GettingTrackingEvent_ShouldReturnEvent()
    {
      // Arrange
      Event @event = GetTestEvent(3);

      var eventRepository = new EventRepository(_context);

      // Act
      var trackedEvent = eventRepository.GetEvent(3);
      var expectedEvent = eventRepository.GetEvent(3);

      // Assert
      Assert.NotNull(expectedEvent);
    }

    [Fact]
    public void GetCalendarEvents_GettingValidEvents_ShouldReturnEvents()
    {
      // Arrange
      Event[] @events = new Event[]
      {
        GetTestEvent(1),
        GetTestEvent(2),
        GetTestEvent(3)
      };

      foreach (var ev in events)
      {
        ev.CalendarId = 1;
        _context.Update(ev);
        _context.SaveChanges();
      }

      var eventRepository = new EventRepository(_context);

      // Act
      var expectedEvent = eventRepository.GetCalendarEvents(events[0].CalendarId);

      // Assert
      Assert.Equal(expectedEvent, events);
    }

    [Fact]
    public void GetCalendarEvents_GettingInvalidEvents_ShouldReturnEmptyArray()
    {
      // Arrange
      int calendarId = int.MinValue;

      var eventRepository = new EventRepository(_context);

      // Act
      var expectedEvent = eventRepository.GetCalendarEvents(calendarId);

      // Assert
      Assert.Equal(expectedEvent, new Event[] { });
    }

    [Fact]
    public void GetMainEvent_GettingValidMainEvent_ShouldReturnEvent()
    {
      // Arrange
      Event[] @events = new Event[]
      {
        GetTestEvent(1),
        GetTestEvent(2),
        GetTestEvent(3)
      };

      foreach (var ev in @events)
      {
        ev.SeriesId = 1;
        _context.Update(ev);
        _context.SaveChanges();
      }

      var eventRepository = new EventRepository(_context);

      // Act
      var expectedEvent = eventRepository.GetMainEvent(@events[2].Id);

      // Assert
      Assert.Equal(expectedEvent, events[0]);
    }

    [Fact]
    public void GetMainEvent_GettingInvalidMainEvent_ShouldThrowException()
    {
      EventRepository eventRepository = new EventRepository(_context);
 
      // Act and Assert
      Assert.Throws<InvalidOperationException>(() => eventRepository.GetMainEvent(123));
    }

    [Fact]
    public void GetSeries_GettingValidSeries_ShouldReturnEventSeriesArray()
    {
      // Arrange
      Event[] @events = new Event[]
      {
        GetTestEvent(1),
        GetTestEvent(2),
        GetTestEvent(3)
      };
        GetTestEvent(4);

      foreach (var ev in @events)
      {
        ev.SeriesId = 1;
        _context.Update(ev);
        _context.SaveChanges();
      }

      var eventRepository = new EventRepository(_context);

      // Act
      var expectedEvent = eventRepository.GetSeries(1);

      // Assert
      Assert.Equal(expectedEvent, @events);
    }

    [Fact]
    public void GetMainEvent_GettingInvalidSeries_ShouldReturnEmptyArray()
    {
      // Arrange
      var eventRepository = new EventRepository(_context);

      // Act
      var expectedEvent = eventRepository.GetSeries(-1);

      // Assert
      Assert.Equal(expectedEvent, new Event[] { });
    }

    [Fact]
    public void GetEventInfo_GettingValidEvent_ShouldReturnUserEventDTO()
    {
      // Arrange
      Event @event = GetTestEvent(3);

      UserEventDTO @userEvent = new UserEventDTO()
      {
        Reiteration = @event.Reiteration,
        UserId = @event.Calendar.User.Id
      };

      var eventRepository = new EventRepository(_context);

      // Act
      var expectedEvent = eventRepository.GetEventInfo(3);

      // Assert
      Assert.Equal(expectedEvent.UserId, @userEvent.UserId);
      Assert.Equal(expectedEvent.Reiteration, @userEvent.Reiteration);
    }

    [Fact]
    public void GetEventInfo_GettingInvalidEvent_ShouldReturnNull()
    {
      // Arrange
      var eventRepository = new EventRepository(_context);

      // Act
      var expectedEvent = eventRepository.GetEventInfo(-1);

      // Assert
      Assert.Null(expectedEvent);
    }

    [Fact]
    public void AddSeriesOfCalendarEvents_AddingSeries_ShouldAddEventSeriesToDB()
    {
      // Arrange
      Event[] @events = new Event[]
      {
        GetTestEvent(1),
        GetTestEvent(2),
        GetTestEvent(3)
      };

      foreach (var ev in @events)
      {
        ev.Id = default;
        ev.SeriesId = 1;
      }

      var eventRepository = new EventRepository(_context);

      // Act
      eventRepository.AddSeriesOfCalendarEvents(@events);

      // Assert
      Assert.NotNull(_context.Events.Where(ev => ev.SeriesId == 1).ToList());
    }

    [Fact]
    public void AddSeriesOfCalendarEvents_AddingEmptySeries_ShouldAddEmptyEventSeriesToDB()
    {
      // Arrange
      Event[] @events = new Event[] { };

      var eventRepository = new EventRepository(_context);

      // Act
      eventRepository.AddSeriesOfCalendarEvents(@events);

      // Assert
      Assert.Equal(_context.Events.Where(ev => ev.SeriesId == 1), @events);
    }

    [Fact]
    public void AddCalendarEvent_AddingEvent_ShouldAddEventToDB()
    {
      // Arrange
      Event @event = GetTestEvent(1);
      @event.Id = default;

      var eventRepository = new EventRepository(_context);

      // Act
      var expectedEvent = eventRepository.AddCalendarEvent(@event);

      // Assert
      Assert.Equal(_context.Events.Find(@event.Id), @event);
    }

    [Fact]
    public void AddCalendarEvent_AddingEventWithExistingId_ShouldThrowException()
    {
      // Arrange
      Event @event = GetTestEvent(1);

      var eventRepository = new EventRepository(_context);

      // Act and Assert
      Assert.Throws<ArgumentException>(() => eventRepository.AddCalendarEvent(@event));
    }

    [Fact]
    public void UpdateCalendarEvent_UpdatingEvent_ShouldUpdateInDB()
    {
      // Arrange
      Event @event = GetTestEvent(1);
      @event.Venue = "Home";

      var eventRepository = new EventRepository(_context);

      // Act
      Event expectedEvent = eventRepository.UpdateCalendarEvent(@event);

      // Assert
      Assert.Equal(expectedEvent, @event);
    }

    [Fact]
    public void UpdateCalendarEvent_UpdatingNotExistingEvent_ShouldThrowException()
    {
      // Arrange
      Event @event = GetTestEvent(1);
      @event.Id = -9;

      var eventRepository = new EventRepository(_context);

      // Act and Assert
      Assert.Throws<InvalidOperationException>(() => eventRepository.UpdateCalendarEvent(@event));
    }

    [Fact]
    public void DeleteCalendarEvent_RemovingExistingEvent_ShouldRemoveFromDB()
    {
      // Arrange
      Event @event = GetTestEvent(1);

      var eventRepository = new EventRepository(_context);

      // Act
      eventRepository.DeleteCalendarEvent(@event.Id);

      // Assert
      Assert.Null(_context.Events.Find(@event.Id));
    }

    [Fact]
    public void DeleteCalendarEvent_RemovingNotExistingEvent_ShouldThrowException()
    {
      // Arrange
      var eventRepository = new EventRepository(_context);

      // Act and Assert
      Assert.Throws<ArgumentNullException>(() => eventRepository.DeleteCalendarEvent(-10));
    }

    [Fact]
    public void DeleteCalendarEventSeries_RemovingExistingEventSeries_ShouldRemoveSeries()
    {
      // Arrange
      int seriesId = 1;
      Event[] @events = new Event[]
      {
          GetTestEvent(1),
          GetTestEvent(2),
          GetTestEvent(3)
      };

      foreach (var ev in @events)
      {
        ev.SeriesId = seriesId;
        _context.Update(ev);
        _context.SaveChanges();
      }

      var eventRepository = new EventRepository(_context);

      //Act
      eventRepository.DeleteCalendarEventSeries(seriesId);

      // Assert
      Assert.Equal(_context.Events.Where(ev => ev.SeriesId == seriesId).ToArray(), new Event[] { });
    }

    [Fact]
    public void DeleteCalendarEventSeries_RemovingNotExistingEventSeries_ShouldThrowException()
    {
      // Arrange
      var eventRepository = new EventRepository(_context);

      // Act and Assert
      Assert.Throws<InvalidOperationException>(() => eventRepository.DeleteCalendarEventSeries(-1));
    }

    [Fact]
    public void UpdateCalendarEventSeries_UpdatingExistingEventSeries_ShouldUpdateEventSeriesInDB()
    {
      // Arrange
      Event[] @events = new Event[]
       {
          GetTestEvent(1),
          GetTestEvent(2),
          GetTestEvent(3)
       };

      foreach (var ev in @events)
      {
        ev.SeriesId = 1;
        _context.Update(ev);
        _context.SaveChanges();
      }
      @events[2].Name = "123";
      var updatedEvent = @events[2];
      _context.Update(updatedEvent);
      _context.SaveChanges();

      var eventRepository = new EventRepository(_context);

      // Act
      var expectedSeries = eventRepository.UpdateCalendarEventSeries(updatedEvent);

      //Arrange

      foreach (var ev in @events)
      {
        ev.Name = "123";
        _context.Update(ev);
        _context.SaveChanges();
      }

      // Assert
      Assert.Equal(expectedSeries, @events);
    }

    [Fact]
    public void UpdateCalendarEventSeries_UpdatingNotExistingEventSeries_ShouldThrowException()
    {
      // Arrange
      Event @events = null;
      var eventRepository = new EventRepository(_context);

      // Assert
      Assert.Throws<NullReferenceException>(() => eventRepository.UpdateCalendarEventSeries(@events));
    }
  }
}
