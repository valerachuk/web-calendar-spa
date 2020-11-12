using System;
using System.Collections.Generic;
using System.Text;

namespace WebCalendar.Data.Entities
{
  public class Log
  {
    public int Id { get; set; }
    public DateTime DateTime { get; set; }
    public string Message { get; set; }
    public string Level { get; set; }
  }
}
