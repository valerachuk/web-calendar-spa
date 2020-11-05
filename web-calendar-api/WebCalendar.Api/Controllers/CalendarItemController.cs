using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebCalendar.Business.Domains.Interfaces;

namespace WebCalendar.Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
  public class CalendarItemController : ControllerBase
  {
    private readonly ICalendarItemDomain _itDomain;
    public CalendarItemController(ICalendarItemDomain calendarItemDomain)
    {
      _itDomain = calendarItemDomain;
    }
    [HttpGet]
    public IActionResult GetCalendarsItems([FromQuery] DateTime[] timeInterval, [FromQuery] int[] id)
    {
      var items = _itDomain.GetCalendarsItemsByTimeInterval(timeInterval[0], timeInterval[1], id);
      return Ok(items);
    }
  }
}
