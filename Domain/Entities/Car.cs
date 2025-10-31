using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Domain.Entities;

public class Car : BaseEntities
{
    [Required]
    public required string Brand { get; set; }
    public required string Model { get; set; }
    public int Year { get; set; }
    public decimal DailyPrice { get; set; }
    public string? ImagePath { get; set; }
    public bool IsAvailable { get; set; }
    public List<Rental>? Rentals { get; set; }
    
}