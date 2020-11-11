using AutoMapper;
using System.Collections.Generic;
using System.Linq;
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

      var seriesId = @event.SeriesId.GetValueOrDefault();
      if (@event.Reiteration != null)
      {
        GenerateEventsOfSeries(calendarEvent, seriesId);
      }

      if (@event.NotificationTime != null)
      {
        _notificationSender.ScheduleEventSeriesStartedNotification(seriesId);
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

      _notificationSender.NotifyEventDeleted(id, false);
      var @event = _evRepository.DeleteCalendarEvent(id);
      _notificationSender.CancelScheduledNotification(@event);
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

      _notificationSender.NotifyEventDeleted(id, true);
      var eventSeries = _evRepository.DeleteCalendarEventSeries(id);
      _notificationSender.CancelScheduledNotification(eventSeries.ToArray());
    }
  }
}
