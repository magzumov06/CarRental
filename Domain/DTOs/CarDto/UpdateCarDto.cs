namespace Domain.DTOs.CarDto;

public class UpdateCarDto
{
    public int Id { get; set; }
    public required string Brand { get; set; }
    public required string Model { get; set; }
    public int Year { get; set; }
    public decimal DailyPrice { get; set; }
    public string? ImagePath { get; set; }
    public bool IsAvailable { get; set; }
}