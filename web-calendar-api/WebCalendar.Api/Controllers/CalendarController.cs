using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebCalendar.Api.Extensions;
using WebCalendar.Business.Domains;
using WebCalendar.Business.Domains.Interfaces;
using WebCalendar.Business.ViewModels;

namespace WebCalendar.Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class CalendarController : ControllerBase
  {
    private readonly ICalendarDomain _caDomain;

    public CalendarController(ICalendarDomain calendarDomain)
    {
      _caDomain = calendarDomain;
    }

    [HttpGet("{id}")]
    public IActionResult GetUserCalendars(int id) => Ok(_caDomain.GetUserCalendars(id));

    [HttpPost]
    public IActionResult AddCalendar(CalendarViewModel calendar)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      int id = _caDomain.AddCalendar(calendar);
      if (id > 0)
      {
        calendar.Id = id;
        return Ok(calendar);
      }

      return BadRequest();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteCalendar(int id)
    {
      if (_caDomain.DeleteCalendar(id, User.GetId()))
        return Ok(id);

      return BadRequest();
    }

  }
}
