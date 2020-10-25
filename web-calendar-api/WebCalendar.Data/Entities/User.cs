using System.Collections.Generic;

namespace WebCalendar.Data.Entities
{
  public class User
  {
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] Salt { get; set; }
    public List<Calendar> Calendars { get; set; }

    public User()
		{
      this.Calendars = new List<Calendar>();
		}
  }
}
