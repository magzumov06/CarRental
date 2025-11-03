using Domain.Enums;

namespace Domain.DTOs.RentalDto;

public class UpdateRentalDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CarId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public Status Status { get; set; }
}