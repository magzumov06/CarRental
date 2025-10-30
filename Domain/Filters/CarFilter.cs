namespace Domain.Filters;

public class CarFilter : BaseFilter
{
    public int? Id { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }
    public decimal? DailyPrice { get; set; }
    public bool? IsAvailable { get; set; }
}