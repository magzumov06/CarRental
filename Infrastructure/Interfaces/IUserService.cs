using Domain.DTOs.UserDto;
using Domain.Filters;
using Domain.Responces;

namespace Infrastructure.Interfaces;

public interface IUserService
{
    Task<Responce<string>> UpdateUser(UpdateUserDto dto);
    Task<Responce<string>> DeleteUser(int id);
    Task<Responce<GetUserDto>> GetUserById(int id);
    Task<PaginationResponce<List<GetUserDto>>> GetAllUsers(UserFilter filter);
    
}