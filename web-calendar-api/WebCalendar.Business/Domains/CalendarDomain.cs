using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using NLog;
using WebCalendar.Business.Domains.Interfaces;
using WebCalendar.Business.DTO;
using WebCalendar.Business.Exceptions;
using WebCalendar.Business.ViewModels;
using WebCalendar.Data.Entities;
using WebCalendar.Data.Repositories.Interfaces;

namespace WebCalendar.Business.Domains
{
  public class CalendarDomain : ICalendarDomain
  {
    private readonly ICalendarRepository _caRepository;
    private readonly IEventRepository _eventRepository;
    private readonly INotificationSenderDomain _notificationSender;
    private readonly IMapper _mapper;
    private Logger _logger;

    public CalendarDomain(
      ICalendarRepository calendarRepository,
      IMapper mapper,
      INotificationSenderDomain notificationSender,
      IEventRepository eventRepository
      )
    {
      _eventRepository = eventRepository;
      _caRepository = calendarRepository;
      _mapper = mapper;
      _notificationSender = notificationSender;
      _logger = LogManager.GetCurrentClassLogger();
    }

    public IEnumerable<CalendarViewModel> GetUserCalendars(int id) =>
      _mapper.Map<IEnumerable<Calendar>, IEnumerable<CalendarViewModel>>(_caRepository.GetUserCalendars(id));

    public Calendar GetCalendar(int id)
    {
      var calendar = _caRepository.GetCalendar(id);
      if (calendar == null)
        throw new NotFoundException("Calendar not found");

      return calendar;
    }

    public int AddCalendar(CalendarViewModel calendar, int userId)
    {
      if (calendar.UserId != userId)
        throw new ForbiddenException("Not calendar owner");

      return _caRepository.AddCalendar(_mapper.Map<CalendarViewModel, Calendar>(calendar));
    }

    public bool DeleteCalendar(int id, int userId) 
    {
      if (GetCalendar(id).UserId != userId)
        throw new ForbiddenException("Not calendar owner");

      _notificationSender.CancelScheduledNotification(_eventRepository.GetCalendarEvents(id).ToArray());

      _logger.Info($"Delete calendar {id}");
      return _caRepository.DeleteCalendar(id);
    }

    public bool EditCalendar(CalendarViewModel calendarView, int userId)
    {
      if (GetCalendar(calendarView.Id).UserId != userId)
        throw new ForbiddenException("Not calendar owner");

      return _caRepository.EditCalendar(_mapper.Map<CalendarViewModel, Calendar>(calendarView));
    }

    public CalendarICSDTO CreateICS(int calendarId, int userId)
    {
      var calendar = _caRepository.GetCalendarWithEvents(calendarId);

      if (calendar.UserId != userId)
        throw new ForbiddenException("Not calendar owner");

      var icsCalendar = new Ical.Net.Calendar();
      icsCalendar.Events.AddRange(calendar.Events.Select(evt =>
      {
        var icsEvent = new CalendarEvent
        {
          Summary = evt.Name,
          Location = evt.Venue,
          Start = new CalDateTime(evt.StartDateTime),
          End = new CalDateTime(evt.EndDateTime)
        };

        if (evt.NotificationTime != null)
        {
          icsEvent.Alarms.Add(new Alarm
          {
            Trigger = new Trigger(TimeSpan.FromMinutes(-(int)evt.NotificationTime))
          });
        }

        return icsEvent;
      }));

      return new CalendarICSDTO
      {
        ICSContent = new CalendarSerializer(icsCalendar).SerializeToString(),
        CalendarName = calendar.Name
      };
    }
  }
}
