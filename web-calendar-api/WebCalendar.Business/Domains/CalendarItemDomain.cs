using AutoMapper;
using System;
using System.Collections.Generic;
using WebCalendar.Business.Domains.Interfaces;
using WebCalendar.Business.ViewModels;
using WebCalendar.Data.Entities;
using WebCalendar.Data.Repositories.Interfaces;

namespace WebCalendar.Business.Domains
{
  public class CalendarItemDomain: ICalendarItemDomain
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
      List<CalendarItemViewModel> calendarItems = new List<CalendarItemViewModel>();
      var calendarEvents = _mapper.Map<IEnumerable<Event>, IEnumerable <EventViewModel>>(
        _itRepository.GetClaendarsEventsByTimeInterval(startDateTime, endDateTime, calendarsId));

      calendarItems.AddRange(_mapper.Map<IEnumerable<EventViewModel>, IEnumerable<CalendarItemViewModel>>(calendarEvents));
      return calendarItems;
    }
  }
}
