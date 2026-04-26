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
    #region MAPPER
    private static GetCarDto MapToDto(Car x)
    {
        return new GetCarDto
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
        };
    }
    #endregion

    #region AddCar
    public async Task<Responce<string>> AddCar(AddCarDto dto)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
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
            
            await transaction.CommitAsync();
            
            return res > 0
                ? new Responce<string>(HttpStatusCode.Created,"Car added successfully")
                : new Responce<string>(HttpStatusCode.BadRequest,"Error");
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            Log.Error("Error in creating car");
            return new Responce<string>(HttpStatusCode.InternalServerError,e.Message);
        }
    }
    #endregion

    #region AddRangeCar
    public async Task<Responce<string>> AddCars(List<AddCarDto> dtos)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        var uploadedFiles = new List<string>();

        try
        {
            Log.Information("Adding multiple cars {@count}", dtos.Count);

            var cars = new List<Car>();

            foreach (var dto in dtos)
            {
                var car = new Car
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
                    var filePath = await file.UploadFile(dto.ImagePath, "CarImage");
                    car.ImagePath = filePath;
                    uploadedFiles.Add(filePath);
                }

                cars.Add(car);
            }

            await context.Cars.AddRangeAsync(cars);
            await context.SaveChangesAsync();

            await transaction.CommitAsync();

            return new Responce<string>(HttpStatusCode.Created, $"{cars.Count} cars added successfully");
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            
            foreach (var path in uploadedFiles)
            {
                await file.DeleteFile(path);
            }

            Log.Error(e, "Error while adding multiple cars");
            return new Responce<string>(HttpStatusCode.InternalServerError, e.Message);
        }
    }
    #endregion
    
    #region UpdateCar
    public async Task<Responce<string>> UpdateCar(UpdateCarDto dto)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            Log.Information("Updating car");
            var car = await context.Cars.FirstOrDefaultAsync(x=> x.Id == dto.Id && !x.IsDeleted);
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
            
            await transaction.CommitAsync();
            
            return res > 0
                ? new Responce<string>(HttpStatusCode.OK,"Car updated successfully")
                : new Responce<string>(HttpStatusCode.BadRequest,"Error");
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            Log.Error("Error in creating car");
            return new Responce<string>(HttpStatusCode.InternalServerError,e.Message);
        }
    }
    #endregion
    
    #region DeleteCar
    public async Task<Responce<string>> DeleteCar(int id)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            Log.Information("Deleting car");
            var car = await context.Cars.FirstOrDefaultAsync(x => x.Id == id &&  !x.IsDeleted);
            
            if(car == null) 
                return new Responce<string>(HttpStatusCode.NotFound,"Car not found");
            
            if (!string.IsNullOrEmpty(car.ImagePath))
                await file.DeleteFile(car.ImagePath);
            
            car.IsDeleted = true;
            car.UpdatedDate = DateTime.UtcNow;
            var res = await context.SaveChangesAsync();
            
            await transaction.CommitAsync();
            return res > 0
                ? new Responce<string>(HttpStatusCode.OK,"Car deleted successfully")
                : new Responce<string>(HttpStatusCode.BadRequest,"Error");
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            Log.Error("Error in deleting car");
            return new Responce<string>(HttpStatusCode.InternalServerError,e.Message);
        }
    }
    #endregion

    #region DeleteCars
    public async Task<Responce<string>> DeleteCars(List<int> ids)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var cars = await context.Cars
                .Where(x => ids.Contains(x.Id) && !x.IsDeleted)
                .ToListAsync();

            var foundIds = cars.Select(x => x.Id).ToList();
            var notFoundIds = ids.Except(foundIds).ToList();

            if (cars.Count == 0)
                return new Responce<string>(HttpStatusCode.NotFound, "Cars not found");

            var filePaths = cars
                .Where(x => !string.IsNullOrEmpty(x.ImagePath))
                .Select(x => x.ImagePath)
                .ToList();

            foreach (var car in cars)
            {
                car.IsDeleted = true;
                car.UpdatedDate = DateTime.UtcNow;
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            foreach (var path in filePaths)
            {
                await file.DeleteFile(path);
            }

            var message = $"Deleted: {cars.Count}";
            if (notFoundIds.Any())
                message += $", Not found: {string.Join(",", notFoundIds)}";

            return new Responce<string>(HttpStatusCode.OK, message);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            Log.Error(e, "Error while deleting multiple cars");

            return new Responce<string>(HttpStatusCode.InternalServerError, e.Message);
        }
    }
    #endregion
    
    #region GetCar
    public async Task<Responce<GetCarDto>> GetCarById(int id)
    {
        try
        {
            Log.Information("Getting car");
            var car = await context.Cars
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            
            if(car == null) 
                return new Responce<GetCarDto>(HttpStatusCode.NotFound,"Car not found");
            
            return new Responce<GetCarDto>(MapToDto(car));
        }
        catch (Exception e)
        {
            Log.Error("Error in getting car");
            return new Responce<GetCarDto>(HttpStatusCode.InternalServerError,e.Message);
        }
    }
    #endregion
    
    #region GetCars
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
            var cars = await query.Skip(skip)
                .Take(filter.PageSize)
                .AsNoTracking()
                .ToListAsync();
            if(cars.Count ==  0) 
                return new PaginationResponce<List<GetCarDto>>(HttpStatusCode.NotFound,"Cars not found");
            
            var result = cars.Select(MapToDto).ToList();
            
            return new PaginationResponce<List<GetCarDto>>(result, total,  filter.PageNumber, filter.PageSize);

        }
        catch (Exception e)
        {
            Log.Error("Error in getting cars");
            return new PaginationResponce<List<GetCarDto>>(HttpStatusCode.InternalServerError,e.Message);
        }
    }
    #endregion
}