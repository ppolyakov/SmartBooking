﻿using System.ComponentModel.DataAnnotations;

namespace SmartBooking.BlazorUI.Models;

public class LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;
}