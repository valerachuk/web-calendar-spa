using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebCalendar.Data.DTO;
using WebCalendar.Data.Entities;
using WebCalendar.Data.Repositories.Interfaces;

namespace WebCalendar.Data.Repositories
{
  public class EventRepository : IEventRepository
  {
    private readonly IWebCalendarDbContext _context;
    public EventRepository(IWebCalendarDbContext context)
    {
      _context = context;
    }

    public Event GetEvent(int id)
    {
      var @event = 
      _context.Events
        .Include(ev => ev.Guests)
        .ThenInclude(eventGuests => eventGuests.User)
        .Where(ev => ev.Id == id)
        .FirstOrDefault();
      if (@event == null)
      {
        return null;
      }
      _context.Entry(@event).State = EntityState.Detached;
      return @event;
    }

    public IEnumerable<Event> GetCalendarEvents(int calendarId)
      => _context.Events.Where(evt => evt.CalendarId == calendarId).ToArray();

    public Event GetMainEvent(int id)
    {
      var seriesEvent = _context.Events.AsNoTracking().FirstOrDefault(e => e.Id == id);

      // find min event id in series for getting main event of the series
      var mainSeriesEvent = _context.Events.Where(ev => ev.SeriesId == seriesEvent.SeriesId);
      int mainEventId = mainSeriesEvent.Min(ev => ev.Id);
      return _context.Events.Find(mainEventId);
    }

    public IEnumerable<Event> GetSeries(int seriesId)
      => _context.Events.Where(evt => evt.SeriesId == seriesId).ToArray();

    public EventNotificationDTO GetEventNotificationInfo(int id) =>
      _context.Events
        .Include(ev => ev.Guests)
        .ThenInclude(eventGuests => eventGuests.User)
        .Where(evt => evt.Id == id)
        .Select(evt => new EventNotificationDTO
        {
          EventName = evt.Name,
          StartDateTime = evt.StartDateTime,
          CalendarName = evt.Calendar.Name,
          UserFirstName = evt.Calendar.User.FirstName,
          UserWantsReceiveEmailNotifications = evt.Calendar.User.ReceiveEmailNotifications,
          IsSeries = evt.Reiteration != null,
          UserEmail = evt.Calendar.User.Email,
          Guests = evt.Guests
        })
        .FirstOrDefault();

    public UserEventDTO GetEventInfo(int id)
    {
      UserEventDTO @event = _context.Events
        .Where(evt => evt.Id == id)
        .Select(evt => new UserEventDTO
        {
          Reiteration = evt.Reiteration,
          UserId = evt.Calendar.UserId
        })
        .FirstOrDefault();
      if (@event == null)
        return null;
      _context.Entry(_context.Events.Find(id)).State = EntityState.Detached;
      return @event;
    }

    public void AddSeriesOfCalendarEvents(IEnumerable<Event> calendarEvents)
    {
      _context.Events.AddRange(calendarEvents);
      _context.SaveChanges();
    }

    public Event AddCalendarEvent(Event calendarEvent)
    {
      _context.Events.Add(calendarEvent);
      _context.SaveChanges();

      return calendarEvent;
    }

    public Event DeleteCalendarEvent(int calendarEventId)
    {
      var currentEvent = _context.Events.Find(calendarEventId);
      if (currentEvent == null)
      {
        return null;
      }
      _context.Events.Remove(currentEvent);
      _context.SaveChanges();
      return currentEvent;
    }

    public IEnumerable<Event> DeleteCalendarEventSeries(int calendarEventId)
    {
      Event currentEvent = _context.Events.Find(calendarEventId);
      var eventSeries = _context.Events.Where(ev => ev.SeriesId == currentEvent.SeriesId).ToArray();
      _context.Events.RemoveRange(eventSeries);
      _context.SaveChanges();
      return eventSeries;
    }

    private void UpdateGuests(Event calendarEvent)
    {
      var oldEventGuests = _context.EventGuests.Where(x => x.EventId == calendarEvent.Id).ToList();
      _context.EventGuests.RemoveRange(oldEventGuests);
      _context.SaveChanges();
      _context.EventGuests.AddRange(calendarEvent.Guests);
      _context.SaveChanges();
    }

    public Event UpdateCalendarEvent(Event calendarEvent)
    {
      calendarEvent.SeriesId = _context.Events.AsNoTracking().FirstOrDefault(e => e.Id == calendarEvent.Id).SeriesId;
      _context.Events.Update(calendarEvent);
      UpdateGuests(calendarEvent);
      _context.SaveChanges();
      return calendarEvent;
    }

    public IEnumerable<Event> UpdateCalendarEventSeries(Event calendarEvent)
    {
      var currentEvent = _context.Events.Find(calendarEvent.Id);
      var currentEventSeries = _context.Events
        .Include(ev => ev.Guests)
        .ThenInclude(eventGuests => eventGuests.User)
        .Where(ev => ev.SeriesId == currentEvent.SeriesId).ToList();

      var oldEventGuests = _context.EventGuests.Where(x => x.EventId == calendarEvent.Id).ToList();
      if (oldEventGuests.Count > 0)
      {
        _context.EventGuests.RemoveRange(oldEventGuests);
        _context.SaveChanges();
      }

      foreach (var item in currentEventSeries)
      {
        item.Calendar = calendarEvent.Calendar;
        item.CalendarId = calendarEvent.CalendarId;
        item.Name = calendarEvent.Name;
        item.Venue = calendarEvent.Venue;
        item.NotificationTime = calendarEvent.NotificationTime;
        item.StartDateTime = item.StartDateTime.Date
          + new TimeSpan(calendarEvent.StartDateTime.Hour, calendarEvent.StartDateTime.Minute, 0);
        item.EndDateTime = item.EndDateTime.Date
          + new TimeSpan(calendarEvent.EndDateTime.Hour, calendarEvent.EndDateTime.Minute, 0);

        // set new guest list to the event in the series 
        item.Guests = calendarEvent.Guests.Select(x => { x.EventId = item.Id; x.Event = item; return x; }).ToList();
        _context.EventGuests.AddRange(item.Guests);
      }
      _context.SaveChanges();
      return currentEventSeries;
    }

    public void UnsubscribeSharedEvent(int id, int guestId)
    {
      var currentEvent = _context.Events.Find(id);
      currentEvent.Guests.Remove(_context.EventGuests.Where(eg => eg.UserId == guestId && eg.EventId == id).FirstOrDefault());
      var oldEventGuests = _context.EventGuests.Where(x => x.UserId == guestId);
      _context.EventGuests.RemoveRange(oldEventGuests);
      _context.Events.Update(currentEvent);
      _context.SaveChanges();
    }

    public void UnsubscribeSharedEventSeries(int id, int guestId)
    {
      var currentEvent = _context.Events.Find(id);
      var currentEventSeries = _context.Events.Where(ev => ev.SeriesId == currentEvent.SeriesId).ToList();
      foreach (var item in currentEventSeries)
      {
        item.Guests.Remove(_context.EventGuests.Where(eg => eg.UserId == guestId && eg.EventId == id).FirstOrDefault());
        var oldEventGuests = _context.EventGuests.Where(x => x.UserId == guestId);
        _context.EventGuests.RemoveRange(oldEventGuests);
        _context.Events.Update(currentEvent);
      }
      _context.SaveChanges();
    }
  }
}
