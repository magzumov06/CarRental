namespace Domain.DTOs.RentalDto;

public class GetRentalDto : UpdateRentalDto
{
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}