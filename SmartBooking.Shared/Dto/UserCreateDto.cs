namespace SmartBooking.Shared.Dto;

public class UserCreateDto
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public bool IsAdmin { get; set; }
}