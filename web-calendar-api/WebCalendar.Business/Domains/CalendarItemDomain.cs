using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using WebCalendar.Business.Domains.Interfaces;
using WebCalendar.Business.ViewModels;
using WebCalendar.Constants.Enums;
using WebCalendar.Data.Entities;
using WebCalendar.Data.Repositories.Interfaces;

namespace WebCalendar.Business.Domains
{
  public class CalendarItemDomain : ICalendarItemDomain
  {
    private readonly IMapper _mapper;
    private readonly ICalendarItemRepository _itRepository;
    private readonly INotificationSenderDomain _notificationSender;

    public CalendarItemDomain(ICalendarItemRepository calendarItemRepository, IMapper mapper, INotificationSenderDomain notificationSender)
    {
      _itRepository = calendarItemRepository;
      _mapper = mapper;
      _notificationSender = notificationSender;
    }
    public IEnumerable<CalendarItemViewModel> GetCalendarsItemsByTimeInterval(DateTime startDateTime, DateTime endDateTime, int[] calendarsId, int userId)
    {
      // get events
      List<CalendarItemViewModel> сalendarItems = _mapper.Map<IEnumerable<Event>, IEnumerable<CalendarItemViewModel>>(
        _itRepository.GetCalendarsEventsByTimeInterval(startDateTime, endDateTime, calendarsId)).ToList();

      // get shared events
      сalendarItems.AddRange(_mapper.Map<IEnumerable<Event>, IEnumerable<CalendarItemViewModel>>(
        _itRepository.GetSharedCalendarEventsByTimeInterval(startDateTime, endDateTime, calendarsId, userId)));

      return сalendarItems;
    }
    public void UpdateCalendarsItem(CalendarItemViewModel calendarItem)
    {
      switch (calendarItem.MetaType)
      {
        case CalendarItemType.Event:
          _itRepository.UpdateCalendarsEventTime(calendarItem.StartDateTime, calendarItem.EndDateTime, calendarItem.Id);
          _notificationSender.ScheduleEventEditedNotification(calendarItem.Id);
          break;
        case CalendarItemType.RepeatableEvent:
          _itRepository.UpdateCalendarsEventTime(calendarItem.StartDateTime, calendarItem.EndDateTime, calendarItem.Id);
          _notificationSender.ScheduleEventEditedNotification(calendarItem.Id);
          break;
        case CalendarItemType.Task:

          break;
        case CalendarItemType.Reminder:

          break;
        default:
          break;
      }
    }
  }
}
