using System.Net;
using Domain.DTOs.RentalDto;
using Domain.Entities;
using Domain.Filters;
using Domain.Responces;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Infrastructure.Services;

public class RentalService(DataContext context) : IRentalService
{
    
    public async Task<Responce<string>> CreateRental(CreateRentalDto dto)
    {
        try
        {
            Log.Information("Creating rental");
            var userExists = await context.Users.AnyAsync(x => x.Id == dto.UserId);
            if (!userExists)
                return new Responce<string>(HttpStatusCode.NotFound, "User not found");

            var car = await context.Cars.FirstOrDefaultAsync(x => x.Id == dto.CarId);
            if (car == null)
                return new Responce<string>(HttpStatusCode.NotFound, "Car not found");
            
            if (dto.StartDate > dto.EndDate)
                return new Responce<string>(HttpStatusCode.BadRequest, "End date must be after start date");
            
            var days = (dto.EndDate - dto.StartDate).TotalDays;
            if(days < 1) days = 1;

            var totalPrice = (decimal)days * car.DailyPrice;

            var newRental = new Rental
            {
                UserId = dto.UserId,
                CarId = dto.CarId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                TotalPrice = totalPrice,
                Status = dto.Status,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };
            await context.Rentals.AddAsync(newRental);
            await context.SaveChangesAsync();
            return new Responce<string>(HttpStatusCode.Created, "Rental has been created successfully");
        }
        catch (Exception e)
        {
            Log.Error("Error creating rental");
            return new Responce<string>(HttpStatusCode.InternalServerError, e.Message);
        }
    }
    
    
    public async Task<Responce<string>> UpdateRental(UpdateRentalDto dto)
    {
        try
        {
            Log.Information("Updating rental");
            var rental = await context.Rentals.FirstOrDefaultAsync(x=> x.Id == dto.Id);
            if (rental == null) return new Responce<string>(HttpStatusCode.NotFound, "Rental not found");
            rental.CarId = dto.CarId;
            rental.StartDate = dto.StartDate;
            rental.EndDate = dto.EndDate;
            rental.TotalPrice = dto.TotalPrice;
            rental.Status = dto.Status;
            rental.UpdatedDate = DateTime.UtcNow;
            var res = await context.SaveChangesAsync();
            return res > 0
                ? new Responce<string>(HttpStatusCode.OK, "Rental has been updated")
                : new Responce<string>(HttpStatusCode.BadRequest, "Rental could not be updated");
        }
        catch (Exception e)
        {
            Log.Error("Error updating rental");
            return new Responce<string>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    public async Task<Responce<string>> DeleteRental(int id)
    {
        try
        {
            Log.Information("Deleting rental");
            var rental = await context.Rentals.FirstOrDefaultAsync(x => x.Id == id);
            if (rental == null) return new Responce<string>(HttpStatusCode.NotFound, "Rental not found");
            context.Rentals.Remove(rental);
            var res = await context.SaveChangesAsync();
            return res > 0
                ? new Responce<string>(HttpStatusCode.OK, "Rental has been deleted")
                : new Responce<string>(HttpStatusCode.BadRequest, "Rental could not be deleted");
        }
        catch (Exception e)
        {
            Log.Error("Error deleting rental");
            return new Responce<string>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    public async Task<Responce<GetRentalDto>> GetRental(int id)
    {
        try
        {
            Log.Information("Getting rental");
            var rental = await context.Rentals.FirstOrDefaultAsync(x => x.Id == id);
            if(rental == null)  return new Responce<GetRentalDto>(HttpStatusCode.NotFound, "Rental not found");
            var dto = new GetRentalDto()
            {
                Id = rental.Id,
                CarId = rental.CarId,
                UserId = rental.UserId,
                StartDate = rental.StartDate,
                EndDate = rental.EndDate,
                TotalPrice = rental.TotalPrice,
                Status = rental.Status,
                CreatedDate = rental.CreatedDate,
                UpdatedDate = rental.UpdatedDate,
            };
            return new Responce<GetRentalDto>(dto);
        }
        catch (Exception e)
        {
            Log.Error("Error getting rental");
            return new Responce<GetRentalDto>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    public async Task<PaginationResponce<List<GetRentalDto>>> GetRentals(RentalFilter filter)
    {
        try
        {
            var query = context.Rentals.AsQueryable();
            if (filter.Id.HasValue)
            {
                query = query.Where(x => x.Id == filter.Id.Value);
            }

            if (filter.CarId.HasValue)
            {
                query = query.Where(x => x.CarId == filter.CarId.Value);
            }

            if (filter.TotalPrice.HasValue)
            {
                query = query.Where(x => x.TotalPrice >= filter.TotalPrice.Value);
            }

            if (filter.Status.HasValue)
            {
                query = query.Where(x => x.Status == filter.Status.Value);
            }
            
            var total = await query.CountAsync();
            var skip = (filter.PageNumber - 1) * filter.PageSize;
            var rental = await query.OrderBy(x => x.Id).Skip(skip).Take(filter.PageSize).ToListAsync();
            if(rental.Count == 0) return new PaginationResponce<List<GetRentalDto>>(HttpStatusCode.NotFound, "Rental not found");
            var dtos = rental.Select(x => new GetRentalDto()
            {
                Id = x.Id,
                CarId = x.CarId,
                UserId = x.UserId,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                TotalPrice = x.TotalPrice,
                Status = x.Status,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate,
            }).ToList();
            return new PaginationResponce<List<GetRentalDto>>(dtos,total,filter.PageNumber,filter.PageSize);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}