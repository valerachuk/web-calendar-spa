using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using WebCalendar.Data;
using WebCalendar.Data.Entities;
using WebCalendar.Data.Repositories;
using Xunit;

namespace WebCalendar.UnitTests.Data
{
  public class CalendarItemRepositoryTest : IDisposable
  {
    private readonly WebCalendarDbContext _context;

    public CalendarItemRepositoryTest()
    {
      var options = new DbContextOptionsBuilder();
      options.UseInMemoryDatabase("WebCalendarTestInMemoryDatabase_CalendarItemRepo");

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
        EndDateTime = new DateTime(2020, 5, 7),
        Reiteration = null,
        Calendar = calendar,
        Id = id
      };

      _context.Events.Add(@event);
      _context.SaveChanges();

      return @event;
    }

    [Fact]
    public void GetCalendarsEventsByTimeInterval_GettingAllIncludedEvents_ShouldReturnItemsByFilter()
    {
      // Arrange
      List<Event> expectedItems = new List<Event>()
      {
        GetTestEvent(1),
        GetTestEvent(2),
        GetTestEvent(3)
      };

      DateTime start = new DateTime(2020, 5, 6);
      DateTime end = new DateTime(2020, 5, 10);
      int[] calendarId = new int[] { 1, 2, 3 };

      CalendarItemRepository itemRepository = new CalendarItemRepository(_context);

      // Act
      var actualResult = itemRepository.GetCalendarsEventsByTimeInterval(start, end, calendarId);

      // Assert
      Assert.Equal(expectedItems, actualResult);
    }

    [Fact]
    public void GetCalendarsEventsByTimeInterval_GettingEventsForTheCalendar_ShouldReturnItemsByFilter()
    {
      // Arrange
      List<Event> items = new List<Event>()
      {
        GetTestEvent(1),
        GetTestEvent(2),
        GetTestEvent(3)
      };

      DateTime start = new DateTime(2020, 5, 6);
      DateTime end = new DateTime(2020, 5, 10);
      int[] calendarId = new int[] { 2 };

      CalendarItemRepository itemRepository = new CalendarItemRepository(_context);

      // Act
      var actualResult = itemRepository.GetCalendarsEventsByTimeInterval(start, end, calendarId);

      // Assert
      Assert.Equal(new Event[] { items[1] }, actualResult);
    }

    [Fact]
    public void GetCalendarsEventsByTimeInterval_GettingListWithOutRangeDates_ShouldReturnEmptyArray()
    {
      // Arrange
      List<Event> items = new List<Event>()
      {
        GetTestEvent(1),
        GetTestEvent(2),
        GetTestEvent(3)
      };

      DateTime start = new DateTime(2020, 5, 10);
      DateTime end = new DateTime(2020, 5, 30);
      int[] calendarId = new int[] { 2 };

      CalendarItemRepository itemRepository = new CalendarItemRepository(_context);

      // Act
      var actualResult = itemRepository.GetCalendarsEventsByTimeInterval(start, end, calendarId);

      // Assert
      Assert.Equal(new Event[] { }, actualResult);
    }

    [Fact]
    public void GetCalendarsEventsByTimeInterval_GettingListOfNotExistingCalendar_ShouldReturnEmptyArray()
    {
      // Arrange
      List<Event> items = new List<Event>()
      {
        GetTestEvent(1),
        GetTestEvent(2),
        GetTestEvent(3)
      };

      DateTime start = new DateTime(2020, 5, 10);
      DateTime end = new DateTime(2020, 5, 30);
      int[] calendarId = new int[] { 12 };

      CalendarItemRepository itemRepository = new CalendarItemRepository(_context);

      // Act
      var actualResult = itemRepository.GetCalendarsEventsByTimeInterval(start, end, calendarId);

      // Assert
      Assert.Equal(new Event[] { }, actualResult);
    }

    [Fact]
    public void GetCalendarsEventsByTimeInterval_GettingListWithInvalidDates_ShouldReturnEmptyArray()
    {
      // Arrange
      List<Event> items = new List<Event>()
      {
        GetTestEvent(1),
        GetTestEvent(2),
        GetTestEvent(3)
      };

      DateTime start = new DateTime(2020, 5, 10);
      DateTime end = new DateTime(2020, 5, 1);
      int[] calendarId = new int[] { 12 };

      CalendarItemRepository itemRepository = new CalendarItemRepository(_context);

      // Act
      var actualResult = itemRepository.GetCalendarsEventsByTimeInterval(start, end, calendarId);

      // Assert
      Assert.Equal(new Event[] { }, actualResult);
    }

    [Fact]
    public void GetCalendarsEventsByTimeInterval_GettingListInvalidCalendar_ShouldReturnEmptyArray()
    {
      // Arrange
      List<Event> items = new List<Event>()
      {
        GetTestEvent(1),
        GetTestEvent(2),
        GetTestEvent(3)
      };

      DateTime start = new DateTime(2020, 5, 10);
      DateTime end = new DateTime(2020, 5, 11);
      int[] calendarId = new int[] { -100 };

      CalendarItemRepository itemRepository = new CalendarItemRepository(_context);

      // Act
      var actualResult = itemRepository.GetCalendarsEventsByTimeInterval(start, end, calendarId);

      // Assert
      Assert.Equal(new Event[] { }, actualResult);
    }

    [Fact]
    public void GetCalendarsEventsByTimeInterval_GettingListWithDatesOnBoundaries_ShouldReturnItems()
    {
      // Arrange
      List<Event> items = new List<Event>()
      {
        GetTestEvent(1),
        GetTestEvent(2),
        GetTestEvent(3)
      };

      items[0].StartDateTime = new DateTime(2020, 5, 4, 23, 59, 59);
      items[0].EndDateTime = new DateTime(2020, 5, 5, 0, 0, 0);
      _context.Update(items[0]);

      items[1].StartDateTime = new DateTime(2020, 5, 4, 23, 59, 59);
      items[1].EndDateTime = new DateTime(2020, 5, 7, 0, 0, 0);
      _context.Update(items[1]);

      items[2].StartDateTime = new DateTime(2020, 5, 5, 0, 0, 0);
      items[2].EndDateTime = new DateTime(2020, 5, 6, 0, 0, 0);
      _context.Update(items[2]);
      _context.SaveChanges();

      DateTime start = new DateTime(2020, 5, 5);
      DateTime end = new DateTime(2020, 5, 6);
      int[] calendarId = new int[] { 1, 2, 3 };

      CalendarItemRepository itemRepository = new CalendarItemRepository(_context);

      // Act
      var actualResult = itemRepository.GetCalendarsEventsByTimeInterval(start, end, calendarId);

      // Assert
      Assert.Equal(items, actualResult);
    }

    [Fact]
    public void UpdateCalendarsEventTime_UpdatingEventDates_ShouldUpdateEvent()
    {
      // Arrange
      var @event = GetTestEvent(1);

      DateTime newStartDate = new DateTime(2020, 5, 5);
      DateTime newEndDate = new DateTime(2020, 5, 6);

      CalendarItemRepository itemRepository = new CalendarItemRepository(_context);

      // Act
      itemRepository.UpdateCalendarsEventTime(newStartDate, newEndDate, @event.Id);

      // Assert
      Assert.Equal(newStartDate, _context.Events.Find(@event.Id).StartDateTime);
      Assert.Equal(newEndDate, _context.Events.Find(@event.Id).EndDateTime);
    }

    [Fact]
    public void UpdateCalendarsEventTime_UpdatingNotExistingEvent_ShouldThrowException()
    {
      // Arrange
      var @event = GetTestEvent(1);
      @event.Id = 123;

      DateTime newStartDate = new DateTime(2020, 5, 5);
      DateTime newEndDate = new DateTime(2020, 5, 6);

      CalendarItemRepository itemRepository = new CalendarItemRepository(_context);

      // Act and Assert
      Assert.Throws<NullReferenceException>(() => itemRepository.UpdateCalendarsEventTime(newStartDate, newEndDate, @event.Id));
    }
  }
}
