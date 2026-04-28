using System.Net;
using Domain.DTOs.UserDto;
using Domain.Entities;
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
    
    #region MAPPER
    private static GetUserDto MapToDto(User u)
    {
        return new GetUserDto
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            ProfilePicture = u.ProfilePicture,
            CreatedDate = u.CreatedDate,
            UpdatedDate = u.UpdatedDate
        };
    }
    #endregion
    
    #region UpdateUser
    public async Task<Responce<string>> UpdateUser(UpdateUserDto dto)
    {
        try
        {
            var user = await context.Users.FirstOrDefaultAsync(x=>x.Id == dto.Id && !x.IsDeleted);
            if(user == null) return new Responce<string>(HttpStatusCode.NotFound, "User not found");
            dto.FullName = user.FullName;
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
    #endregion
    
    #region DeleteUser
    public async Task<Responce<string>> DeleteUser(int id)
    {
        try
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if(user == null) return new Responce<string>(HttpStatusCode.NotFound, "User not found");
            user.IsDeleted = true;
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
    #endregion

    #region GetUserById
    public async Task<Responce<GetUserDto>> GetUserById(int id)
    {
        try
        {
            var user =await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x=> x.Id == id && !x.IsDeleted);
            
            if (user == null)
                return new Responce<GetUserDto>(HttpStatusCode.NotFound, "User not found");
            
            return new Responce<GetUserDto>(MapToDto(user));
        }
        catch (Exception e)
        {
            return new Responce<GetUserDto>(HttpStatusCode.InternalServerError, e.Message);
        }
    }
    #endregion

    #region GetUsers
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
            
            query = query.Where(x => !x.IsDeleted);
            
            var total = await query.CountAsync();
            
            var skip = (filter.PageNumber - 1) * filter.PageSize;
            
            var users = await query.OrderBy(x => x.Id)
                .Skip(skip)
                .Take(filter.PageSize)
                .AsNoTracking()
                .ToListAsync();
            
            if (users.Count == 0)
                return new PaginationResponce<List<GetUserDto>>(HttpStatusCode.NotFound, "User not found");
            
            var res = users.Select(MapToDto).ToList();
            return new PaginationResponce<List<GetUserDto>>(res, total,filter.PageNumber, filter.PageSize);
        }
        catch (Exception e)
        {
            return new PaginationResponce<List<GetUserDto>>(HttpStatusCode.InternalServerError, e.Message);
        }
    }
    #endregion
    
}