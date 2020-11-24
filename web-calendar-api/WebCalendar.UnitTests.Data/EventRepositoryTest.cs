using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebCalendar.Data;
using WebCalendar.Data.DTO;
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
      options.UseInMemoryDatabase("WebCalendarTestInMemoryDatabase_EventRepo");

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
        Name =  $"myCalendar",
        User = user
      };

      var @event = new Event
      {
        Name = $"myEvent {id}",
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
      var actualEventNotificationInfo = eventRepository.GetEventNotificationInfo(3);

      // Assert
      Assert.NotNull(actualEventNotificationInfo);
      Assert.Equal(actualEventNotificationInfo.UserFirstName, @event.Calendar.User.FirstName);
      Assert.Equal(actualEventNotificationInfo.UserWantsReceiveEmailNotifications, @event.Calendar.User.ReceiveEmailNotifications);
      Assert.Equal(actualEventNotificationInfo.UserEmail, @event.Calendar.User.Email);
      Assert.Equal(actualEventNotificationInfo.CalendarName, @event.Calendar.Name);
      Assert.Equal(actualEventNotificationInfo.EventName, @event.Name);
      Assert.Equal(actualEventNotificationInfo.StartDateTime, @event.StartDateTime);
      Assert.False(actualEventNotificationInfo.IsSeries);
    }

    [Fact]
    public void GetEventNotificationInfo_GettingInvalidEventInfo_ShouldReturnNull()
    {
      // Arrange
      var eventRepository = new EventRepository(_context);
      GetTestEvent(3);
      // Act
      var actualEventNotificationInfo = eventRepository.GetEventNotificationInfo(125134);

      // Assert
      Assert.Null(actualEventNotificationInfo);
    }

    [Fact]
    public void GetEvent_GettingNotExistingEvent_ShouldReturnNull()
    {
      // Arrange
      int id = 123;
      var eventRepository = new EventRepository(_context);
      GetTestEvent(3);
      // Act 
      var actualResult = eventRepository.GetEvent(id);
      //Assert
      Assert.Null(actualResult);
    }

    [Fact]
    public void GetEvent_GettingEvent_ShouldReturnEvent()
    {
      // Arrange
      Event @event = GetTestEvent(3);

      var eventRepository = new EventRepository(_context);

      // Act
      var actualEvent = eventRepository.GetEvent(3);

      // Assert
      Assert.Equal(@event.Id, actualEvent.Id);
    }

    [Fact]
    public void GetCalendarEvents_GettingEvents_ShouldReturnEvents()
    {
      // Arrange
      var @events = new Event[]
      {
        new Event
        {
          Id = 1,
          Name = "Event 1",
          CalendarId = 1
        },
        new Event
        {
          Id = 2,
          Name = "Event 2",
          CalendarId = 1
        }
      };

      _context.AddRange(events);
      _context.SaveChanges();

      var eventRepository = new EventRepository(_context);

      // Act
      var actualEvent = eventRepository.GetCalendarEvents(1);

      // Assert
      Assert.Collection(actualEvent,
        ev1 => { Assert.Contains("Event 1", ev1.Name); Assert.Equal(1, ev1.Id); },
        ev2 => { Assert.Contains("Event 2", ev2.Name); Assert.Equal(2, ev2.Id); });
    }

    [Fact]
    public void GetCalendarEvents_GettingNotExistingEvents_ShouldReturnEmptyArray()
    {
      // Arrange
      int calendarId = 123;
      var eventRepository = new EventRepository(_context);
      GetTestEvent(3);

      // Act
      var actualEvent = eventRepository.GetCalendarEvents(calendarId);

      // Assert
      Assert.Equal(new Event[] { }, actualEvent);
    }

    [Fact]
    public void GetMainEvent_GettingValidMainEvent_ShouldReturnEvent()
    {
      var @events = new Event[]
      {
        new Event
        {
          Id = 1,
          Name = "Event 1",
          CalendarId = 1,
          SeriesId = 1
        },
        new Event
        {
          Id = 2,
          Name = "Event 2",
          CalendarId = 1,
          SeriesId = 1
        }
      };

      _context.AddRange(events);
      _context.SaveChanges();

      var eventRepository = new EventRepository(_context);

      // Act
      var actualEvent = eventRepository.GetMainEvent(2);

      // Assert
      Assert.Equal(@events[0].Name, actualEvent.Name);
      Assert.Equal(@events[0].Id, actualEvent.Id);
    }

    [Fact]
    public void GetMainEvent_GettingInvalidMainEvent_ShouldThrowException()
    {
      EventRepository eventRepository = new EventRepository(_context);
      Event @event = GetTestEvent(3);
      // Act and Assert
      Assert.Throws<InvalidOperationException>(() => eventRepository.GetMainEvent(123));
    }

    [Fact]
    public void GetSeries_GettingValidSeries_ShouldReturnEventSeriesArray()
    {
      var @events = new Event[]
      {
        new Event
        {
          Id = 1,
          Name = "Event 1",
          CalendarId = 1,
          SeriesId = 1
        },
        new Event
        {
          Id = 2,
          Name = "Event 2",
          CalendarId = 1,
          SeriesId = 1
        }
      };
      
      _context.AddRange(events);
      _context.SaveChanges();

      var otherEventSeriesEvent = new Event
      {
        Id = 3,
        Name = "Event 3",
        CalendarId = 2,
        SeriesId = 2
      };
      _context.Add(otherEventSeriesEvent);
      _context.SaveChanges();

      var eventRepository = new EventRepository(_context);

      // Act
      var actualEvent = eventRepository.GetSeries(1);

      // Assert
      Assert.Collection(actualEvent,
        ev1 => { Assert.Contains("Event 1", ev1.Name); Assert.Equal(1, ev1.Id); },
        ev2 => { Assert.Contains("Event 2", ev2.Name); Assert.Equal(2, ev2.Id); });
    }

    [Fact]
    public void GetMainEvent_GettingNotExistingSeries_ShouldReturnEmptyArray()
    {
      // Arrange
      var eventRepository = new EventRepository(_context);
      // Act
      var actualEvent = eventRepository.GetSeries(10);

      // Assert
      Assert.Equal(new Event[] { }, actualEvent);
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
      var actualEvent = eventRepository.GetEventInfo(3);

      // Assert
      Assert.Equal(@userEvent.UserId, actualEvent.UserId);
      Assert.Equal(@userEvent.Reiteration, actualEvent.Reiteration);
    }

    [Fact]
    public void GetEventInfo_GettingNotExistingEvent_ShouldReturnNull()
    {
      // Arrange
      var eventRepository = new EventRepository(_context);
      // Act
      var actualEvent = eventRepository.GetEventInfo(123);

      // Assert
      Assert.Null(actualEvent);
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
      eventRepository.AddCalendarEvent(@event);

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
      Event actualEvent = eventRepository.UpdateCalendarEvent(@event);

      // Assert
      Assert.Equal(@event, actualEvent);
    }

    [Fact]
    public void UpdateCalendarEvent_UpdatingNotExistingEvent_ShouldThrowException()
    {
      // Arrange
      Event @event = GetTestEvent(1);
      @event.Id = -9;

      var eventRepository = new EventRepository(_context);

      // Act and Assert
      Assert.Throws<NullReferenceException>(() => eventRepository.UpdateCalendarEvent(@event));
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
    public void DeleteCalendarEvent_RemovingNotExistingEvent_ShouldReturnNull()
    {
      // Arrange
      var eventRepository = new EventRepository(_context);

      // Act and Assert
      Assert.Null(eventRepository.DeleteCalendarEvent(10));
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

      Assert.Equal(events, _context.Events);

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
      Assert.Null(eventRepository.DeleteCalendarEventSeries(10));
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
      var actualSeries = eventRepository.UpdateCalendarEventSeries(updatedEvent);

      // Arrange

      foreach (var ev in @events)
      {
        ev.Name = "123";
        _context.Update(ev);
        _context.SaveChanges();
      }

      // Assert
      Assert.Equal(@events, actualSeries);
    }

    [Fact]
    public void UpdateCalendarEventSeries_UpdatingNotExistingEventSeries_ShouldThrowException()
    {
      // Arrange
      Event @events = null;
      var eventRepository = new EventRepository(_context);
      GetTestEvent(3);
      // Act and Assert
      Assert.Throws<NullReferenceException>(() => eventRepository.UpdateCalendarEventSeries(@events));
    }
  }
}
