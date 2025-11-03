using Domain.Enums;

namespace Domain.DTOs.RentalDto;

public class CreateRentalDto
{
    public int UserId { get; set; }
    public int CarId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Status Status { get; set; }
}