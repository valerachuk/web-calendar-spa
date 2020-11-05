using System;

namespace WebCalendar.Business.Exceptions
{
  public class NotFoundException : Exception
  {
    public NotFoundException(string message) : base(message)
    { }
  }
}
