using AutoMapper;
using System;
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
    private readonly IFileDomain _fileDomain;

    public EventDomain(IEventRepository eventRepository, IMapper mapper, INotificationSenderDomain notificationSender, IFileDomain fileDomain)
    {
      _evRepository = eventRepository;
      _mapper = mapper;
      _notificationSender = notificationSender;
      _fileDomain = fileDomain;
    }

    public EventViewModel GetEvent(int id)
    {
      var currentEvent = _evRepository.GetWholeEvent(id);
      if (currentEvent == null)
      {
        throw new NotFoundException("Event not found");
      }
      return _mapper.Map<Event, EventViewModel>(currentEvent);
    }

    public int AddCalendarEvent(EventViewModel calendarEvent, bool isUpdated = false)
    {
      int id = _evRepository.AddCalendarEvent(_mapper.Map<EventViewModel, Event>(calendarEvent)).Id;
      var @event = _evRepository.GetEvent(id);
      if (isUpdated)
        _notificationSender.ScheduleEventEditedNotification(@event.Id);
      else
        _notificationSender.ScheduleEventCreatedNotification(@event.Id);

      var seriesId = @event.SeriesId.GetValueOrDefault();
      if (@event.Reiteration != null)
      {
        GenerateEventsOfSeries(calendarEvent, seriesId);

        if (@event.NotificationTime != null)
        {
          _notificationSender.ScheduleEventSeriesStartedNotification(seriesId);
        }
      }
      else if (@event.NotificationTime != null && @event.NotificationScheduleJobId == null)
      {
        _notificationSender.ScheduleEventStartedNotification(@event.Id);
      }

      return @event.Id;
    }

    private void GenerateEventsOfSeries(EventViewModel calendarEvent, int seriesId)
    {
      var generatedEvents = new List<Event>();
      int eventRepetitionsNumber = 365;
      int eventFrequency = calendarEvent.Reiteration != null ? (int)calendarEvent.Reiteration : 1;
      calendarEvent.FileId = null;

      // Create events in one series for a selected time interval for the year ahead
      for (int i = eventFrequency; i < eventRepetitionsNumber; i += eventFrequency)
      {
        Event newCalendarEvent = _mapper.Map<EventViewModel, Event>(calendarEvent);
        newCalendarEvent.SeriesId = seriesId;
        newCalendarEvent.StartDateTime = calendarEvent.StartDateTime.AddDays(i);
        newCalendarEvent.EndDateTime = calendarEvent.EndDateTime.AddDays(i);

        generatedEvents.Add(newCalendarEvent);
      }
      _evRepository.AddSeriesOfCalendarEvents(generatedEvents);
    }

    public void UpdateCalendarEvent(EventViewModel calendarEvent, int userId)
    {
      CheckRightsToModify(calendarEvent.Id, userId);
      var oldEvent = _evRepository.GetEvent(calendarEvent.Id);
      _notificationSender.CancelScheduledNotification(oldEvent);
      var oldReiteration = _evRepository.GetEventInfo(calendarEvent.Id).Reiteration;
      if (oldReiteration == calendarEvent.Reiteration)
      {
        _evRepository.UpdateCalendarEvent(_mapper.Map(calendarEvent, oldEvent));
        _notificationSender.ScheduleEventEditedNotification(oldEvent.Id);

       oldEvent = _evRepository.GetEvent(calendarEvent.Id);
        if (oldEvent.NotificationTime != null)
          _notificationSender.ScheduleEventStartedNotification(oldEvent.Id);
      }
      else
      {
        _evRepository.DeleteCalendarEvent(calendarEvent.Id);
        calendarEvent.Id = default;
        AddCalendarEvent(calendarEvent, true);
      }
    }

    public void UpdateCalendarEventSeries(EventViewModel calendarEvent, int userId)
    {
      CheckRightsToModify(calendarEvent.Id, userId);

      var oldReiteration = _evRepository.GetEventInfo(calendarEvent.Id).Reiteration;
      if (oldReiteration == calendarEvent.Reiteration)
      {
        var eventSeries = _evRepository.UpdateCalendarEventSeries(_mapper.Map<EventViewModel, Event>(calendarEvent))
          .ToArray();
        _notificationSender.CancelScheduledNotification(eventSeries);
        _notificationSender.ScheduleEventSeriesStartedNotification(eventSeries.First().SeriesId.GetValueOrDefault());
        _notificationSender.ScheduleEventEditedNotification(calendarEvent.Id);
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
        var eventSeries = _evRepository.GetSeries(mainSeriesEvent.SeriesId.GetValueOrDefault()).ToArray();
        _notificationSender.CancelScheduledNotification(eventSeries);
        _evRepository.DeleteCalendarEventSeries(calendarEvent.Id);
        //add new event series
        updatedMainSeriesEvent.Id = default;
        AddCalendarEvent(_mapper.Map<Event, EventViewModel>(updatedMainSeriesEvent), true);
      }
    }

    public void DeleteCalendarEvent(int id, int userId)
    {
      CheckRightsToModify(id, userId);

      _notificationSender.NotifyEventDeleted(id, false);
      var @event = _evRepository.DeleteCalendarEvent(id);
      if(@event.FileId != null)
        _fileDomain.DeleteFile((int)@event.FileId);
      _notificationSender.CancelScheduledNotification(@event);
    }

    public void DeleteCalendarEventSeries(int id, int userId)
    {
      CheckRightsToModify(id, userId);

      _notificationSender.NotifyEventDeleted(id, true);
      var eventSeries = _evRepository.DeleteCalendarEventSeries(id);
      _notificationSender.CancelScheduledNotification(eventSeries.ToArray());
    }

    public void UnsubscribeSharedEvent(int id, int guestId)
    {
      var currentEvent = _evRepository.GetEventInfo(id);
      if (currentEvent == null)
      {
        throw new NotFoundException("Event not found");
      }
      _evRepository.UnsubscribeSharedEvent(id, guestId);
    }
    public void UnsubscribeSharedEventSeries(int id, int guestId)
    {
      var currentEvent = _evRepository.GetEventInfo(id);
      if (currentEvent == null)
      {
        throw new NotFoundException("Event not found");
      }
      _evRepository.UnsubscribeSharedEventSeries(id, guestId);
    }

    private void CheckRightsToModify(int id, int userId)
    {
      var currentEvent = _evRepository.GetEventInfo(id);
      if (currentEvent == null)
      {
        throw new NotFoundException("Event not found");
      }
      if (userId != currentEvent.UserId)
      {
        throw new ForbiddenException("Not event owner");
      }
    }
  }
}
