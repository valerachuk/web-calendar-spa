using System;

namespace WebCalendar.Data.Entities
{
  public class UserFile
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string UniqueName { get; set; }
    public string Path { get; set; }
    public string Type { get; set; }
    public long Size { get; set; }
    public DateTime UploadDate { get; set; }
    public Event Event { get; set; }
  }
}
