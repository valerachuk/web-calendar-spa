using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebCalendar.Api.Extensions;
using WebCalendar.Business.Domains.Interfaces;
using WebCalendar.Business.ViewModels;

namespace WebCalendar.Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    private readonly IUserDomain _userDomain;

    public AuthController(IUserDomain userDomain)
    {
      _userDomain = userDomain;
    }

    [HttpGet("id")]
    [Authorize]
    public IActionResult GetId()
    {
      return Ok(User.GetId());
    }

    [HttpPost("sign-in")]
    public IActionResult SignIn(LoginViewModel login)
    {
      var id = _userDomain.Authenticate(login);
      if (id == null)
      {
        return UnprocessableEntity();
      }

      return Ok(new
      {
        access_token = _userDomain.GenerateJWT((int)id)
      });
    }

    [HttpPost("sign-up")]
    public IActionResult SignUp(RegisterViewModel register)
    {

      if (_userDomain.HasUser(register.Email))
      {
        return Conflict();
      }

      var id = _userDomain.Register(register);

      return Ok(new
      {
        access_token = _userDomain.GenerateJWT(id)
      });
    }

  }
}
