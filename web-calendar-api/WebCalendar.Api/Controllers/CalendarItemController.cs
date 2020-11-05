using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebCalendar.Business.Domains.Interfaces;
using WebCalendar.Business.ViewModels;

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
    public IActionResult GetCalendarsItems([FromQuery] CalendarItemFilterViewModel filter)
    {
      var items = _itDomain.GetCalendarsItemsByTimeInterval(filter.Start, filter.End, filter.Id);
      return Ok(items);
    }
  }
}
