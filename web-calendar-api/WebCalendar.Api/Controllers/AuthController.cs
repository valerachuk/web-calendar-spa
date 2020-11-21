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

    [HttpGet]
    [Authorize]
    [Route("GetAllUsers")]
    public IActionResult GetAllUsersExceptCurrent()
    {
      return Ok(_userDomain.GetAllUsersExceptCurrent(User.GetId()));
    }

    [HttpGet("id")]
    [Authorize]
    public IActionResult GetId()
    {
      return Ok(User.GetId());
    }

    private IActionResult GenerateUserInfo(int id)
		{
      return Ok(new
      {
        access_token = _userDomain.GenerateJWT(id)
      });
    }

    [HttpPost("sign-in")]
    public IActionResult SignIn(LoginViewModel login)
    {
      var id = _userDomain.Authenticate(login);
      if (id == null)
      {
        return UnprocessableEntity();
      }

      return GenerateUserInfo((int)id);
    }

    [HttpPost("sign-up")]
    public IActionResult SignUp(RegisterViewModel register)
    {
      if (_userDomain.HasUser(register.Email))
      {
        return Conflict();
      }

      var id = _userDomain.Register(register);

      return GenerateUserInfo(id);
    }

    [HttpPut("edit")]
    public IActionResult EditUser(UserViewModel user)
    {
      if (_userDomain.EditUser(user, User.GetId()))
        return GenerateUserInfo(User.GetId());

      return BadRequest();
    }

  }
}
