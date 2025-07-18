using System.ComponentModel.DataAnnotations;

namespace SmartBooking.Shared.Http.Requests;

public class UserEditRequest
{
    [Required, EmailAddress] 
    public string Email { get; set; } = "";
    [Required, MinLength(6)] 
    public string Password { get; set; } = "";
    public bool IsAdmin { get; set; }
}