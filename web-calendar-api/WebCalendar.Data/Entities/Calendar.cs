using System.Collections.Generic;

namespace WebCalendar.Data.Entities
{
  public class Calendar
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public string Description { get; set; }
    public List<Event> Events { get; set; }

    public Calendar()
    {
      this.Events = new List<Event>();
    }
  }
}
