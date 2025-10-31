using System.Net;
using Domain.DTOs.CarDto;
using Domain.Entities;
using Domain.Filters;
using Domain.Responces;
using Infrastructure.Data;
using Infrastructure.FileStorage;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class CarService(DataContext context,
    IFileStorage file) : ICarService
{
    public async Task<Responce<string>> AddCar(AddCarDto dto)
    {
        try
        {
            var newCar = new Car()
            {
                Brand = dto.Brand,
                Model = dto.Model,
                Year = dto.Year,
                DailyPrice = dto.DailyPrice,
                IsAvailable = dto.IsAvailable,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
            };
            if (dto.ImagePath != null)
            {
                newCar.ImagePath = await file.UploadFile(dto.ImagePath,"CarImage");
            }
            await context.Cars.AddAsync(newCar);
            var res = await context.SaveChangesAsync();
            return res > 0
                ? new Responce<string>(HttpStatusCode.Created,"Car added successfully")
                : new Responce<string>(HttpStatusCode.BadRequest,"Error");
        }
        catch (Exception e)
        {
            return new Responce<string>(HttpStatusCode.InternalServerError,"Error");
        }
    }

    public async Task<Responce<string>> UpdateCar(UpdateCarDto dto)
    {
        try
        {
            var car = await context.Cars.FirstOrDefaultAsync(x=> x.Id == dto.Id);
            if(car == null) return new Responce<string>(HttpStatusCode.NotFound,"Car not found");
            car.Brand = dto.Brand;
            car.Model = dto.Model;
            car.Year = dto.Year;
            car.DailyPrice = dto.DailyPrice;
            car.IsAvailable = dto.IsAvailable;
            if (dto.ImagePath != null)
            {
                if (!string.IsNullOrEmpty(car.ImagePath))
                {
                    await file.DeleteFile(car.ImagePath);
                }
                car.ImagePath = await file.UploadFile(dto.ImagePath,"CarImage");
            }
            car.UpdatedDate = DateTime.UtcNow;
            var res = await context.SaveChangesAsync();
            return res > 0
                ? new Responce<string>(HttpStatusCode.OK,"Car updated successfully")
                : new Responce<string>(HttpStatusCode.BadRequest,"Error");
        }
        catch (Exception e)
        {
            return new Responce<string>(HttpStatusCode.InternalServerError,"Error");
        }
    }

    public async Task<Responce<string>> DeleteCar(int id)
    {
        try
        {
            var car = await context.Cars.FirstOrDefaultAsync(x => x.Id == id);
            if(car == null) return new Responce<string>(HttpStatusCode.NotFound,"Car not found");
            context.Cars.Remove(car);
            var res = await context.SaveChangesAsync();
            return res > 0
                ? new Responce<string>(HttpStatusCode.OK,"Car deleted successfully")
                : new Responce<string>(HttpStatusCode.BadRequest,"Error");
        }
        catch (Exception e)
        {
            return new Responce<string>(HttpStatusCode.InternalServerError,"Error");
        }
    }

    public async Task<Responce<GetCarDto>> GetCarById(int id)
    {
        try
        {
            var car = await context.Cars.FirstOrDefaultAsync(x => x.Id == id);
            if(car == null) return new Responce<GetCarDto>(HttpStatusCode.NotFound,"Car not found");
            var res = new GetCarDto()
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
                Year = car.Year,
                DailyPrice = car.DailyPrice,
                ImagePath = car.ImagePath,
                IsAvailable = car.IsAvailable,
                CreatedDate = car.CreatedDate,
                UpdatedDate = car.UpdatedDate,
            };
            return new Responce<GetCarDto>(res);
        }
        catch (Exception e)
        {
            return new Responce<GetCarDto>(HttpStatusCode.InternalServerError,"Error");
        }
    }

    public async Task<PaginationResponce<GetCarDto>> GetCars(CarFilter filter)
    {
        throw new NotImplementedException();
    }
}