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
      return _mapper.Map<Event, EventViewModel>(_evRepository.GetEvent(id).Item1);
    }
    public void AddCalendarEvent(EventViewModel calendarEvent)
    {
      var seriesId = AddMainEventOfSeries(calendarEvent);
      if (seriesId != null)
      {
        GenerateEventsOfSeries(calendarEvent, seriesId.GetValueOrDefault());
      }
    }

    public void UpdateCalendarEvent(EventViewModel calendarEvent, int userId)
    {
      CheckRightsToModify(calendarEvent.Id, userId);
      var oldReiteration = _evRepository.GetEvent(calendarEvent.Id).Item1.Reiteration;
      if (oldReiteration == calendarEvent.Reiteration)
      {
        _evRepository.UpdateCalendarEvent(_mapper.Map<EventViewModel, Event>(calendarEvent));
      }
      else
      {
        _evRepository.DeleteCalendarEvent(calendarEvent.Id);
        calendarEvent.Id = default;
        AddCalendarEvent(calendarEvent);
      }
    }

    public void UpdateCalendarEventSeries(EventViewModel calendarEvent, int userId)
    {
      CheckRightsToModify(calendarEvent.Id, userId);

      var oldReiteration = _evRepository.GetEvent(calendarEvent.Id).Item1.Reiteration;
      if (oldReiteration == calendarEvent.Reiteration)
      {
        _evRepository.UpdateCalendarEventSeries(_mapper.Map<EventViewModel, Event>(calendarEvent));
      }
      else
      {
        // find main (first) event of the series
        var mainSeriesEvent = _evRepository.GetMainEvent(calendarEvent.Id);
        // update main event, except dates 
        var updatedMainSeriesEvent = _evRepository.UpdateCalendarEvent(_mapper.Map<EventViewModel, Event>(calendarEvent));
        updatedMainSeriesEvent.StartDateTime = mainSeriesEvent.StartDateTime.Date
          + new TimeSpan(calendarEvent.StartDateTime.Hour, calendarEvent.StartDateTime.Minute, 0);
        updatedMainSeriesEvent.EndDateTime = mainSeriesEvent.EndDateTime.Date
          + new TimeSpan(calendarEvent.EndDateTime.Hour, calendarEvent.EndDateTime.Minute, 0);
        // delete series
        _evRepository.DeleteCalendarEventSeries(calendarEvent.Id);
        //add new event series
        updatedMainSeriesEvent.Id = default;
        AddCalendarEvent(_mapper.Map<Event, EventViewModel>(updatedMainSeriesEvent));
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

    public void DeleteCalendarEvent(int id, int userId)
    {
      CheckRightsToModify(id, userId);
      _evRepository.DeleteCalendarEvent(id);
    }

    public void DeleteCalendarEventSeries(int id, int userId)
    {
      CheckRightsToModify(id, userId);
      _evRepository.DeleteCalendarEventSeries(id);
    }

    private void CheckRightsToModify(int id, int userId)
    {
      var currentEvent = _evRepository.GetEvent(id);
      if (currentEvent == null)
      {
        throw new NotFoundException("Event not found");
      }
      if (userId != currentEvent.Item2)
      {
        throw new ForbiddenException("Not event owner");
      }
    }
  }
}
