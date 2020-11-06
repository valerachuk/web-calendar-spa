using AutoMapper;
using System;
using System.Collections.Generic;
using WebCalendar.Business.Domains.Interfaces;
using WebCalendar.Business.Exceptions;
using WebCalendar.Business.ViewModels;
using WebCalendar.Data.Entities;
using WebCalendar.Data.Repositories.Interfaces;

namespace WebCalendar.Business.Domains
{
  public class EventDomain : IEventDomain
  {
    private readonly IEventRepository _evRepository;
    private readonly IMapper _mapper;

    public EventDomain(IEventRepository eventRepository, IMapper mapper)
    {
      _evRepository = eventRepository;
      _mapper = mapper;
    }
    public EventViewModel GetEvent(int id)
    {
      return _mapper.Map<Event, EventViewModel>(_evRepository.GetEvent(id));
    }
    public void AddCalendarEvent(EventViewModel calendarEvent)
    {
      var seriesId = AddMainEventOfSeries(calendarEvent);
      if (seriesId != null)
      {
        GenerateEventsOfSeries(calendarEvent, seriesId.GetValueOrDefault());
      }
    }

    private int? AddMainEventOfSeries(EventViewModel calendarEvent)
    {
      return _evRepository.AddCalendarEvents(_mapper.Map<EventViewModel, Event>(calendarEvent));
    }

    private void GenerateEventsOfSeries(EventViewModel calendarEvent, int seriesId)
    {
      var generatedEvents = new List<Event>();
      int eventRepetitionsNumber = 365;
      int eventFrequency = calendarEvent.Reiteration != null ? (int)calendarEvent.Reiteration : 1;

      // Create events in one series for a selected time interval for the year ahead
      for (int i = eventFrequency; i < eventRepetitionsNumber; i += eventFrequency)
      {
        Event newCalendarEvent = _mapper.Map<EventViewModel, Event>(calendarEvent);
        newCalendarEvent.SeriesId = seriesId;
        newCalendarEvent.StartDateTime = calendarEvent.StartDateTime.AddDays(i);
        newCalendarEvent.EndDateTime = calendarEvent.EndDateTime.AddDays(i);

        generatedEvents.Add(newCalendarEvent);
      }
      _evRepository.AddSeriesOfCalendarEvents(generatedEvents, seriesId);
    }

    public void DeleteCalendarEvent(int id, IEnumerable<int> calendarsId)
    {
      try
      {
        _evRepository.DeleteCalendarEvent(id, calendarsId);
      }
      catch (KeyNotFoundException)
      {
        throw new NotFoundException("Event not found");
      }
      catch (UnauthorizedAccessException)
      {
        throw new ForbiddenException("Not event owner");
      }
    }

    public void DeleteCalendarEventSeries(int id, IEnumerable<int> calendarsId)
    {
      try
      {
        _evRepository.DeleteCalendarEventSeries(id, calendarsId);
      }
      catch (KeyNotFoundException)
      {
        throw new NotFoundException("Event not found");
      }
      catch (UnauthorizedAccessException)
      {
        throw new ForbiddenException("Not event owner");
      }

    }
  }
}
