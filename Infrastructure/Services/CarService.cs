using System.Net;
using Domain.DTOs.CarDto;
using Domain.Entities;
using Domain.Filters;
using Domain.Responces;
using Infrastructure.Data;
using Infrastructure.FileStorage;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Infrastructure.Services;

public class CarService(DataContext context,
    IFileStorage file) : ICarService
{
    
    public async Task<Responce<string>> AddCar(AddCarDto dto)
    {
        try
        {
            Log.Information("Adding car");
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
            Log.Error("Error in creating car");
            return new Responce<string>(HttpStatusCode.InternalServerError,e.Message);
        }
    }

    public async Task<Responce<string>> UpdateCar(UpdateCarDto dto)
    {
        try
        {
            Log.Information("Updating car");
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
            Log.Error("Error in creating car");
            return new Responce<string>(HttpStatusCode.InternalServerError,e.Message);
        }
    }
    public async Task<Responce<string>> DeleteCar(int id)
    {
        try
        {
            Log.Information("Deleting car");
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
            Log.Error("Error in deleting car");
            return new Responce<string>(HttpStatusCode.InternalServerError,e.Message);
        }
    }

    public async Task<Responce<GetCarDto>> GetCarById(int id)
    {
        try
        {
            Log.Information("Getting car");
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
            Log.Error("Error in getting car");
            return new Responce<GetCarDto>(HttpStatusCode.InternalServerError,e.Message);
        }
    }

    public async Task<PaginationResponce<List<GetCarDto>>> GetCars(CarFilter filter)
    {
        try
        {
            Log.Information("Getting cars");
            var query = context.Cars.AsQueryable();
            if (filter.Id.HasValue)
            {
                query = query.Where(x => x.Id == filter.Id);
            }

            if (!string.IsNullOrEmpty(filter.Brand))
            {
                query = query.Where(x => x.Brand.Contains(filter.Brand));
            }

            if (!string.IsNullOrEmpty(filter.Model))
            {
                query = query.Where(x => x.Model.Contains(filter.Model));
            }

            if (filter.Year.HasValue)
            {
                query = query.Where(x => x.Year == filter.Year);
            }

            if (filter.DailyPrice.HasValue)
            {
                query = query.Where(x => x.DailyPrice == filter.DailyPrice);
            }

            if (filter.IsAvailable.HasValue)
            {
                query = query.Where(x => x.IsAvailable == filter.IsAvailable);
            }

            query = query.Where(x => x.IsDeleted == false);
            var total = await query.CountAsync();
            var skip = (filter.PageNumber - 1) * filter.PageSize;
            var cars = await query.Skip(skip).Take(filter.PageSize).ToListAsync();
            if(cars.Count ==  0) return new PaginationResponce<List<GetCarDto>>(HttpStatusCode.NotFound,"Cars not found");
            var dtos = cars.Select(x=> new  GetCarDto()
            {
                Id = x.Id,
                Brand = x.Brand,
                Model = x.Model,
                Year = x.Year,
                DailyPrice = x.DailyPrice,
                ImagePath = x.ImagePath,
                IsAvailable = x.IsAvailable,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate,
            }).ToList();
            return new PaginationResponce<List<GetCarDto>>(dtos, total,filter.PageNumber, filter.PageSize);
        }
        catch (Exception e)
        {
            Log.Error("Error in getting cars");
            return new PaginationResponce<List<GetCarDto>>(HttpStatusCode.InternalServerError,e.Message);
        }
    }
}