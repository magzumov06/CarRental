using Domain.DTOs.CarDto;
using Domain.Filters;
using Domain.Responces;

namespace Infrastructure.Interfaces;

public interface ICarService
{
    Task<Responce<string>> AddCar(AddCarDto dto);
    Task<Responce<string>> AddCars(List<AddCarDto> dtos);
    Task<Responce<string>> UpdateCar(UpdateCarDto dto);
    Task<Responce<string>> DeleteCar(int id);
    Task<Responce<string>> DeleteCars(List<int> ids);
    Task<Responce<GetCarDto>> GetCarById(int id);
    Task<PaginationResponce<List<GetCarDto>>> GetCars(CarFilter filter);
}