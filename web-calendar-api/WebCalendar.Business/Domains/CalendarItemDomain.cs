﻿using AutoMapper;
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

    public CalendarItemDomain(ICalendarItemRepository calendarItemRepository, IMapper mapper)
    {
      _itRepository = calendarItemRepository;
      _mapper = mapper;
    }
    public IEnumerable<CalendarItemViewModel> GetCalendarsItemsByTimeInterval(DateTime startDateTime, DateTime endDateTime, int[] calendarsId)
    {
      IEnumerable<CalendarItemViewModel> сalendarItems = _mapper.Map<IEnumerable<Event>, IEnumerable<CalendarItemViewModel>>(
        _itRepository.GetCalendarsEventsByTimeInterval(startDateTime, endDateTime, calendarsId));

      return сalendarItems;
    }
    public void UpdateCalendarsItem(CalendarItemViewModel calendarItem)
    {
      switch (calendarItem.MetaType)
      {
        case CalendarItemType.Event:
          _itRepository.UpdateCalendarsEventTime(calendarItem.StartDateTime, calendarItem.EndDateTime, calendarItem.Id);
          break;
        case CalendarItemType.RepeatableEvent:
          _itRepository.UpdateCalendarsEventTime(calendarItem.StartDateTime, calendarItem.EndDateTime, calendarItem.Id);
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
