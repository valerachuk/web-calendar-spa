using System.ComponentModel.DataAnnotations;

namespace WebCalendar.Business.ViewModels
{
  public class UserViewModel
  {
    [Required(ErrorMessage = "Id is required")]
    public int Id { get; set; }
    [Required(ErrorMessage = "FirstName is required")]
    public string FirstName { get; set; }
    [Required(ErrorMessage = "LastName is required")]
    public string LastName { get; set; }
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }
    [Required(ErrorMessage = "ReceiveEmailNotifications is required")]
    public bool ReceiveEmailNotifications { get; set; }
  }
}
