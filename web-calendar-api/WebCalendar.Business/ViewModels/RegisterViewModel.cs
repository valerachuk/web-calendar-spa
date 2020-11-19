﻿using System.ComponentModel.DataAnnotations;

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
    public string Password { get; set; }
  }
}
