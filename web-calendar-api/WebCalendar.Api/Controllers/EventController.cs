using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebCalendar.Api.Extensions;
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
    private readonly INotificationSenderDomain _notificationSender;
    private readonly ICalendarDomain _calendarDomain;

    public EventController(IEventDomain eventDomain, INotificationSenderDomain notificationSender, ICalendarDomain calendarDomain)
    {
      _evDomain = eventDomain;
      _notificationSender = notificationSender;
      _calendarDomain = calendarDomain;
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

      var userId = User.GetId();
      _notificationSender.NotifyEventCreated(calendarEvent.Name, userId);

      return Ok(calendarEvent);
    }

    [HttpDelete]
    public IActionResult DeleteEvent(int id)
    {
      var userCalendarsId = _calendarDomain.GetUserCalendars(User.GetId()).Select(c => c.Id);
      _evDomain.DeleteCalendarEvent(id, userCalendarsId);
      return Ok();
    }

    [HttpDelete]
    [Route("DeleteSeries")]
    public IActionResult DeleteEventSeries([FromQuery] int id)
    {
      var userCalendarsId = _calendarDomain.GetUserCalendars(User.GetId()).Select(c => c.Id);
      _evDomain.DeleteCalendarEventSeries(id, userCalendarsId);
      return Ok();
    }
  }
}