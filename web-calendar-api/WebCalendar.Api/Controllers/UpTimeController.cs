using Microsoft.AspNetCore.Mvc;

namespace WebCalendar.Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class UpTimeController : ControllerBase
  {
    [HttpGet]
    public IActionResult Get() => Ok();
  }
}
