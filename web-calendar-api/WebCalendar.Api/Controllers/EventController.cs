using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using WebCalendar.Business.Domains;
using WebCalendar.Business.Domains.Interfaces;
using WebCalendar.Business.ViewModels;

namespace WebCalendar.Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class EventController : ControllerBase
  {
    private readonly IEventDomain _evDomain;

    public EventController(IEventDomain eventDomain)
    {
      _evDomain = eventDomain;
    }

    [HttpPost]
    public IActionResult AddCalendarEvent(EventViewModel calendarEvent)
    {
      var totalTime = calendarEvent.EndDateTime - calendarEvent.StartDateTime;

      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      if (calendarEvent.EndDateTime < calendarEvent.StartDateTime)
      {
        return BadRequest(JsonConvert.SerializeObject("End date/time was less than start date/time"));
      }
      if (calendarEvent.Reiteration != null && totalTime.TotalDays > (int)calendarEvent.Reiteration)
      {
        return BadRequest(JsonConvert.SerializeObject("Reiteration must be less or equal to the time interval"));
      }
      _evDomain.AddCalendarEvent(calendarEvent);
      return Ok(calendarEvent);
    }
  }
}