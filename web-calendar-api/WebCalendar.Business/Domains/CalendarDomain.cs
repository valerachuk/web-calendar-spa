using System.Collections.Generic;
using AutoMapper;
using WebCalendar.Business.Domains.Interfaces;
using WebCalendar.Business.Exceptions;
using WebCalendar.Business.ViewModels;
using WebCalendar.Data.Entities;
using WebCalendar.Data.Repositories.Interfaces;

namespace WebCalendar.Business.Domains
{
  public class CalendarDomain : ICalendarDomain
  {
    private readonly ICalendarRepository _caRepository;
    private readonly IMapper _mapper;

    public CalendarDomain(ICalendarRepository calendarRepository, IMapper mapper)
    {
      _caRepository = calendarRepository;
      _mapper = mapper;
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

    public int AddCalendar(CalendarViewModel calendar)
    {
      return _caRepository.AddCalendar(_mapper.Map<CalendarViewModel, Calendar>(calendar));
    }

    public bool DeleteCalendar(int id, int userId) 
    {
      if (_caRepository.GetCalendar(id).UserId != userId)
        throw new ForbiddenException("Not calendar owner");

      return _caRepository.DeleteCalendar(id);
    }

    public bool EditCalendar(CalendarViewModel calendarView, int userId)
    {
      if (GetCalendar(calendarView.Id).UserId != userId)
        throw new ForbiddenException("Not calendar owner");

      return _caRepository.EditCalendar(_mapper.Map<CalendarViewModel, Calendar>(calendarView));
    }
  }
}
