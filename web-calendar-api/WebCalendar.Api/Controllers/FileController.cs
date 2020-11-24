using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebCalendar.Business.Domains.Interfaces;
using WebCalendar.Business.Exceptions;

namespace WebCalendar.Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
  public class FileController : ControllerBase
  {
    private readonly IFileDomain _fileDomain;
    public FileController(IFileDomain fileDomain)
    {
      _fileDomain = fileDomain;
    }

    [HttpGet("{eventId}")]
    public IActionResult GetEventFile(int eventId)
    {
      var fileView = _fileDomain.GetEventFile(eventId);
      if (fileView == null)
        throw new NotFoundException("Event file not found");

      Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
      FileStream stream = new FileStream(fileView.Path, FileMode.Open);
      return File(stream, fileView.Type, fileView.Name);
    }

		[HttpPost]
		public IActionResult UploadFile(IFormCollection form)
    {
      var fileId = _fileDomain.AddFile(form.Files[0]);
      if (fileId > 0)
			  return Ok(fileId);

      return BadRequest();
		}
  }
}
