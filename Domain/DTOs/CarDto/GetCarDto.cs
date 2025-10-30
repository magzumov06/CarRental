namespace Domain.DTOs.CarDto;

public class GetCarDto : UpdateCarDto
{
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}