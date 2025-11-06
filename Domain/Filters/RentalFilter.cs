using Domain.Enums;

namespace Domain.Filters;

public class RentalFilter : BaseFilter
{
    public int? Id { get; set; }
    public int? CarId { get; set; }
    public decimal? TotalPrice { get; set; }
    public Status? Status { get; set; }
}