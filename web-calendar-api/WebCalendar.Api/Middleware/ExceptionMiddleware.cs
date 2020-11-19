using Microsoft.AspNetCore.Http;
using NLog;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using WebCalendar.Business.Exceptions;

namespace WebCalendar.Api.Middleware
{
  public class ExceptionMiddleware
  {
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
      _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
      try
      {
        await _next(context);
      }
      catch (Exception ex)
      {
        await HandleExceptionAsync(context, ex);
      }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
      HttpStatusCode status;
      string message;

      if (exception is ForbiddenException)
      {
        message = exception.Message;
        status = HttpStatusCode.Forbidden;
      }
      else if(exception is NotFoundException)
      {
        message = exception.Message;
        status = HttpStatusCode.NotFound;
      }
      else
      {
        message = exception.Message;
        status = HttpStatusCode.InternalServerError;
      }

      Logger logger = LogManager.GetCurrentClassLogger();
      logger.Error(message);

      var result = JsonSerializer.Serialize(new { error = message, errorStatus = status });
      context.Response.ContentType = "application/json";
      context.Response.StatusCode = (int)status;
      return context.Response.WriteAsync(result);
    }
  }
}
