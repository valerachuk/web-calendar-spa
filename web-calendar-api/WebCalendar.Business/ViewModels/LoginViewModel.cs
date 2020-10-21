using System.ComponentModel.DataAnnotations;

namespace WebCalendar.Business.ViewModels
{
  public class LoginViewModel
  {
    [Required]
    public string Email { get; set; }
    
    [Required]
    public string Password { get; set; }
  }
}
