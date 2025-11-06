using System.Net;
using Domain.DTOs.UserDto;
using Domain.Filters;
using Domain.Responces;
using Infrastructure.Data;
using Infrastructure.FileStorage;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class UserService(DataContext context,
    IFileStorage file) : IUserService
{
    public async Task<Responce<string>> UpdateUser(UpdateUserDto dto)
    {
        try
        {
            var user = await context.Users.FirstOrDefaultAsync(x=>x.Id == dto.Id);
            if(user == null) return new Responce<string>(HttpStatusCode.NotFound, "User not found");
            dto.FullName = user.FullName;
            dto.Email = user.Email;
            if (dto.ProfilePicture != null)
            {
                if (!string.IsNullOrEmpty(user.ProfilePicture))
                {
                    await file.DeleteFile(user.ProfilePicture);
                }
                await file.UploadFile(dto.ProfilePicture,"UserAvatar");
            }
            var res = await context.SaveChangesAsync();
            return res > 0
                ? new Responce<string>(HttpStatusCode.OK, "User successfully updated")
                : new Responce<string>(HttpStatusCode.NotFound, "User not found");
        }
        catch (Exception e)
        {
            return new Responce<string>(HttpStatusCode.InternalServerError, e.Message);
        }
    }
    
    public async Task<Responce<string>> DeleteUser(int id)
    {
        try
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if(user == null) return new Responce<string>(HttpStatusCode.NotFound, "User not found");
            context.Users.Remove(user);
            var res = await context.SaveChangesAsync();
            return res > 0
                ? new Responce<string>(HttpStatusCode.OK, "User successfully deleted")
                : new Responce<string>(HttpStatusCode.NotFound, "User not found");
        }
        catch (Exception e)
        {
            return new Responce<string>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    public async Task<Responce<GetUserDto>> GetUserById(int id)
    {
        try
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if(user == null) return new Responce<GetUserDto>(HttpStatusCode.NotFound, "User not found");
            var dto = new GetUserDto()
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                ProfilePicture = user.ProfilePicture,
                CreatedDate = user.CreatedDate,
                UpdatedDate = user.UpdatedDate
            };
            return new Responce<GetUserDto>(dto);
        }
        catch (Exception e)
        {
            return new Responce<GetUserDto>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    public async Task<PaginationResponce<List<GetUserDto>>> GetAllUsers(UserFilter filter)
    {
        try
        {
            var query = context.Users.AsQueryable();
            if (filter.Id.HasValue)
            {
                query = query.Where(x => x.Id == filter.Id.Value);
            }

            if (!string.IsNullOrEmpty(filter.FullName))
            {
                query = query.Where(x => x.FullName.Contains(filter.FullName));
            }

            if (!string.IsNullOrEmpty(filter.Email))
            {
                query = query.Where(x => x.Email.Contains(filter.Email));
            }
            var total = await query.CountAsync();
            var skip = (filter.PageNumber - 1) * filter.PageSize;
            var users = await query.OrderBy(x => x.Id).Skip(skip).Take(filter.PageSize).ToListAsync();
            if (users.Count == 0)
                return new PaginationResponce<List<GetUserDto>>(HttpStatusCode.NotFound, "User not found");
            var dtos = users.Select(x=> new GetUserDto()
            {
                Id = x.Id,
                FullName = x.FullName,
                Email = x.Email,
                ProfilePicture = x.ProfilePicture,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate
            }).ToList();
            return new PaginationResponce<List<GetUserDto>>(dtos, total,filter.PageNumber, filter.PageSize);
        }
        catch (Exception e)
        {
            return new PaginationResponce<List<GetUserDto>>(HttpStatusCode.InternalServerError, e.Message);
        }
    }
}