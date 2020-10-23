using Microsoft.AspNetCore.Mvc;
using WebCalendar.Business.Domains;
using WebCalendar.Business.Domains.Interfaces;
using WebCalendar.Business.ViewModels;

namespace WebCalendar.Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class CalendarController : ControllerBase
  {
    private readonly ICalendarDomain _CaDomain;

    public CalendarController(ICalendarDomain calendarDomain)
    {
      _CaDomain = calendarDomain;
    }

    [HttpGet("{id}")]
    public IActionResult GetUserCalendars(int id) => Ok(_CaDomain.GetUserCalendars(id));

    [HttpPost]
    public IActionResult AddCalendar(CalendarViewModel calendar)
    {
      if (string.IsNullOrEmpty(calendar.Name))
        ModelState.AddModelError("Name", "Wrong calendar name");
      else if (calendar.UserId <= 0)
        ModelState.AddModelError("UserId", "UserId is required");

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      int id = _CaDomain.AddCalendar(calendar);
      if (id > 0)
      {
        calendar.Id = id;
        return Ok(calendar);
      }

      return BadRequest();
    }

  }
}
