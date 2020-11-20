using System;
using System.ComponentModel.DataAnnotations;

namespace WebCalendar.Business.ViewModels
{
  public class EventFileViewModel
  {
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    public string UniqueName { get; set; }
    [Required]
    public string Path { get; set; }
    public string Type { get; set; }
    [Required]
    public long Size { get; set; }
    public DateTime UploadDate { get; set; }
    [Required]
    public int EventId { get; set; }
  }
}
