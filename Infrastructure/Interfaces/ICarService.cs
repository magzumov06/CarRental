using Domain.DTOs.CarDto;
using Domain.Filters;
using Domain.Responces;

namespace Infrastructure.Interfaces;

public interface ICarService
{
    Task<Responce<string>> AddCar(AddCarDto dto);
    Task<Responce<string>> UpdateCar(UpdateCarDto dto);
    Task<Responce<string>> DeleteCar(int id);
    Task<Responce<GetCarDto>> GetCarById(int id);
    Task<PaginationResponce<GetCarDto>> GetCars(CarFilter filter);
}