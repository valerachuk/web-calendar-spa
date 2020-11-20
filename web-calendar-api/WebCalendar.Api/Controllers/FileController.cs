using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebCalendar.Business.Domains.Interfaces;
using WebCalendar.Business.Exceptions;

namespace WebCalendar.Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class FileController : ControllerBase
  {
    private readonly IEventFileDomain _efDomain;
    public FileController(IEventFileDomain fileDomain)
    {
      _efDomain = fileDomain;
    }

    [HttpGet("{eventId}")]
    public IActionResult GetEventFile(int eventId)
    {
      var fileView = _efDomain.GetEventFile(eventId);
      if (fileView == null)
        throw new NotFoundException("Event file not found");

      Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
      FileStream stream = new FileStream(fileView.Path, FileMode.Open);
      return File(stream, fileView.Type, fileView.Name);
    }

		[HttpPost]
		public IActionResult UploadFile(IFormCollection form)
    {
      var eventId = int.Parse(form["eventId"]);

      if (_efDomain.AddFile(form.Files[0], eventId) > 0)
			  return Ok();

      return BadRequest();
		}
  }
}
