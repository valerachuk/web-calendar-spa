using Microsoft.AspNetCore.Mvc;
using WebCalendar.Business.Domains;
using WebCalendar.Business.Domains.Interfaces;

namespace WebCalendar.Api.Controllers
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
