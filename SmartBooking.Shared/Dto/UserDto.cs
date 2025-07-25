﻿namespace SmartBooking.Shared.Dto;

public class UserDto
{
    public string Id { get; set; } = "";
    public string Email { get; set; } = "";
    public List<string> Roles { get; set; } = new();
}