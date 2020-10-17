using Microsoft.AspNetCore.Mvc;
using web_calendar_business.Domains;

namespace web_calendar_api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class UserController : ControllerBase
  {
    private readonly IUserDomain _userDomain;
    
    public UserController(IUserDomain userDomain)
    {
      _userDomain = userDomain;
    }

    [HttpGet]
    public IActionResult Get() => Ok(_userDomain.Get());

  }
}
