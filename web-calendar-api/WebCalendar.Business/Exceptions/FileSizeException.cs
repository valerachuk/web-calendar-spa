using System;

namespace WebCalendar.Business.Exceptions
{
  public class FileSizeException : Exception
  {
    public FileSizeException(string message) : base(message)
    { }
  }
}
