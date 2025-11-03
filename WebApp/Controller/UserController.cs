using Domain.DTOs.UserDto;
using Domain.Filters;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controller;
 [ApiController]
 [Route("api/[controller]")]
public class UserController(IUserService service) : ControllerBase
{
    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromForm]UpdateUserDto dto)
    {
        var res =  await service.UpdateUser(dto);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var res = await service.DeleteUser(id);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var res = await service.GetUserById(id);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery]UserFilter filter)
    {
        var res = await service.GetAllUsers(filter);
        return StatusCode((int)res.StatusCode, res);
    }
}