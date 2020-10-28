using System;

namespace WebCalendar.Business.Exceptions
{
  public class ForbiddenException : Exception
  {
    public ForbiddenException(string message) : base(message)
    { }
  }
}
