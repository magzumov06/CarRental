using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Domain.DTOs.CarDto;

public class AddCarDto
{
    public required string Brand { get; set; }
    public required string Model { get; set; }
    [Required]
    [Range(1990 , 2026, ErrorMessage = "Year must be between 1990 and 2026.")]
    public int Year { get; set; }
    public decimal DailyPrice { get; set; }
    public IFormFile? ImagePath { get; set; }
    public bool IsAvailable { get; set; }
}