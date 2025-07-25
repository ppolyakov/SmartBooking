using System.ComponentModel.DataAnnotations;

namespace SmartBooking.Shared.Dto;

public class ServiceDto
{
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; } = "";

    public string Description { get; set; } = "";

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal? Price { get; set; }

    [Required(ErrorMessage = "Date is required")]
    public DateTime? Date { get; set; }

    [Required(ErrorMessage = "Duration is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 minute")]
    public int? Duration { get; set; }
}