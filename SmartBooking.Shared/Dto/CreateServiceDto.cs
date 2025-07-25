using System.ComponentModel.DataAnnotations;

namespace SmartBooking.Shared.Dto;

public class CreateServiceDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 minute")]
    public int Duration { get; set; }

    [Required]
    public DateTime Date { get; set; }

    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be positive")]
    public decimal Price { get; set; }
}