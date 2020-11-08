using AutoMapper;
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
    private readonly INotificationSenderDomain _notificationSender;

    public EventDomain(IEventRepository eventRepository, IMapper mapper, INotificationSenderDomain notificationSender)
    {
      _evRepository = eventRepository;
      _mapper = mapper;
      _notificationSender = notificationSender;
    }

    public EventViewModel GetEvent(int id)
    {
      return _mapper.Map<Event, EventViewModel>(_evRepository.GetEvent(id).Item1);
    }

    public void AddCalendarEvent(EventViewModel calendarEvent)
    {
      var @event = _evRepository.AddCalendarEvents(_mapper.Map<EventViewModel, Event>(calendarEvent));
      _notificationSender.ScheduleEventCreatedNotification(@event.Id);
      _notificationSender.ScheduleEventStartedNotification(@event.Id);

      if (@event.Reiteration != null)
      {
        GenerateEventsOfSeries(calendarEvent, @event.SeriesId.GetValueOrDefault());
      }
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
      generatedEvents.ForEach(evt => _notificationSender.ScheduleEventStartedNotification(evt.Id)); // executes for ~2sec. Will be moved to schedule, that schedules each event of series
    }

    public void DeleteCalendarEvent(int id, int userId)
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
      _evRepository.DeleteCalendarEvent(id);
    }

    public void DeleteCalendarEventSeries(int id, int userId)
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
      _evRepository.DeleteCalendarEventSeries(id);
    }
  }
}
