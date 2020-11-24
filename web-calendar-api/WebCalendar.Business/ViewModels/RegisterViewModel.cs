using System.ComponentModel.DataAnnotations;

namespace WebCalendar.Business.ViewModels
{
  public class RegisterViewModel
  {
    [Required]
    public string FirstName { get; set; }
    
    [Required]
    public string LastName { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$", 
      ErrorMessage = "Password must contain at least one number, one letter and be 8 characters long")]
    public string Password { get; set; }
  }
}
