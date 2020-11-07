using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebCalendar.Business.Domains.Interfaces;
using WebCalendar.Business.ViewModels;

namespace WebCalendar.Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
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

      if (calendarEvent.EndDateTime < calendarEvent.StartDateTime)
      {
        return BadRequest("End date/time was less than start date/time");
      }
      if (calendarEvent.EndDateTime == calendarEvent.StartDateTime)
      {
        return BadRequest("The beginning and end of the event cannot be at the same time");
      }
      if (calendarEvent.Reiteration != null && totalTime.TotalDays > (int)calendarEvent.Reiteration)
      {
        return BadRequest("Reiteration must be less or equal to the time interval");
      }
      _evDomain.AddCalendarEvent(calendarEvent);

      return Ok(calendarEvent);
    }
  }
}
