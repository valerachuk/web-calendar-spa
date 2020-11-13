using System.Collections.Generic;
using AutoMapper;
using NLog;
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
    private Logger _logger;

    public CalendarDomain(ICalendarRepository calendarRepository, IMapper mapper)
    {
      _caRepository = calendarRepository;
      _mapper = mapper;
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

      _logger.Info($"Delete calendar {id}");

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
