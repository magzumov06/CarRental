using Domain.DTOs.RentalDto;
using Domain.Filters;
using Domain.Responces;

namespace Infrastructure.Interfaces;

public interface IRentalService
{
    Task<Responce<string>> CreateRental(CreateRentalDto dto,int userId);
    Task<Responce<string>> UpdateRental(UpdateRentalDto dto, int userId);
    Task<Responce<string>> DeleteRental(int id, int userId);
    Task<Responce<GetRentalDto>> GetRental(int id);
    Task<PaginationResponce<List<GetRentalDto>>> GetRentals(RentalFilter filter);
    Task<PaginationResponce<List<GetRentalDto>>> GetRentalByUserId(int userId, int pageNumber = 1, int pageSize = 10);
    Task MarkExpiredRentalsAsCompleted();
}