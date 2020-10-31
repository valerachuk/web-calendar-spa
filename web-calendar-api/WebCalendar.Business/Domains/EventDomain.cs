using AutoMapper;
using System.Collections.Generic;
using WebCalendar.Business.Domains.Interfaces;
using WebCalendar.Business.ViewModels;
using WebCalendar.Data.Entities;
using WebCalendar.Data.Repositories.Interfaces;

namespace WebCalendar.Business.Domains
{
  public class EventDomain : IEventDomain
  {
    private readonly IEventRepository _evRepository;

    public EventDomain(IEventRepository eventRepository, IMapper mapper)
    {
      _evRepository = eventRepository;
    }

    public void AddCalendarEvent(EventViewModel calendarEvent)
    {
      int seriesId = AddMainEventOfSeries(calendarEvent);
      GenerateEventsOfSeries(calendarEvent, seriesId);
    }

    private int AddMainEventOfSeries(EventViewModel calendarEvent)
    {
      var newCalendarEvent = new Event()
      {
        Name = calendarEvent.Name,
        StartDateTime = calendarEvent.StartDateTime,
        EndDateTime = calendarEvent.EndDateTime,
        Venue = calendarEvent.Venue,
        NotificationTime = calendarEvent.NotificationTime,
        Reiteration = calendarEvent.Reiteration,
        CalendarId = calendarEvent.CalendarId
      };

      return _evRepository.AddCalendarEvents(newCalendarEvent);
    }

    private void GenerateEventsOfSeries(EventViewModel calendarEvent, int seriesId)
    {
      var generatedEvents = new List<Event>();
      int eventRepetitionsNumber = calendarEvent.Reiteration != null ? 365 / (int)calendarEvent.Reiteration : -1;
      int eventFrequency = calendarEvent.Reiteration != null ? (int)calendarEvent.Reiteration : 1;

      // Create events in one series for a selected time interval for the year ahead
      for (int i = eventFrequency; i < eventRepetitionsNumber; i += eventFrequency)
      {
        var newCalendarEvent = new Event()
        {
          Name = calendarEvent.Name,
          StartDateTime = calendarEvent.StartDateTime.AddDays(i),
          EndDateTime = calendarEvent.EndDateTime.AddDays(i),
          Venue = calendarEvent.Venue,
          NotificationTime = calendarEvent.NotificationTime,
          Reiteration = calendarEvent.Reiteration,
          CalendarId = calendarEvent.CalendarId,
          SeriesId = seriesId
        };
        generatedEvents.Add(newCalendarEvent);
      }
      _evRepository.AddSeriesOfCalendarEvents(generatedEvents, seriesId);
    }
  }
}
