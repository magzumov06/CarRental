namespace Domain.Entities;

public class Car : BaseEntities
{
    public string Brand { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public decimal DailyPrice { get; set; }
    public string? ImagePath { get; set; }
    public bool IsAvailable { get; set; }
    
}