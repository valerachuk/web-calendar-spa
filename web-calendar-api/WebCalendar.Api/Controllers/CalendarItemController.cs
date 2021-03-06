﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebCalendar.Api.Extensions;
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
      var items = _itDomain.GetCalendarsItemsByTimeInterval(filter.Start, filter.End, filter.Id, User.GetId());
      return Ok(items);
    }

    [HttpPut]
    public IActionResult PutCalendarsItems(CalendarItemViewModel calendarItem)
    {
      _itDomain.UpdateCalendarsItem(calendarItem);
      return Ok();
    }
  }
}
