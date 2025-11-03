using Domain.DTOs.RentalDto;
using Domain.Responces;

namespace Infrastructure.Interfaces;

public interface IRentalService
{
    Task<Responce<string>> CreateRental(CreateRentalDto dto);
    Task<Responce<string>> UpdateRental(UpdateRentalDto dto);
    Task<Responce<string>> DeleteRental(int id);
    Task<Responce<GetRentalDto>> GetRental(int id);
}