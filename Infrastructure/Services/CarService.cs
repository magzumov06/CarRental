using Domain.DTOs.CarDto;
using Domain.Filters;
using Domain.Responces;
using Infrastructure.Data;
using Infrastructure.FileStorage;
using Infrastructure.Interfaces;

namespace Infrastructure.Services;

public class CarService(DataContext context,
    IFileStorage file) : ICarService
{
    public async Task<Responce<string>> AddCar(AddCarDto dto)
    {
        throw new NotImplementedException();
    }

    public async Task<Responce<string>> UpdateCar(UpdateCarDto dto)
    {
        throw new NotImplementedException();
    }

    public async Task<Responce<string>> DeleteCar(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Responce<GetCarDto>> GetCarById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<PaginationResponce<GetCarDto>> GetCars(CarFilter filter)
    {
        throw new NotImplementedException();
    }
}