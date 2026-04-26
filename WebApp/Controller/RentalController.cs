using System.Security.Claims;
using Domain.DTOs.RentalDto;
using Domain.Filters;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controller;


[ApiController]
[Route("api/[controller]")]
public class RentalController(IRentalService service) :  ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateRental([FromBody] CreateRentalDto dto)
    {
        var userClaim =User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userClaim == null)
            return Unauthorized("User not Authorized");
        var userId = int.Parse(userClaim);
        var res = await service.CreateRental(dto, userId);
        return StatusCode((int)res.StatusCode, res);
    }


    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateRental(UpdateRentalDto dto)
    {
        var userClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userClaim == null)
            return Unauthorized("User not Authorized");
        var userId = int.Parse(userClaim);
        var res = await service.UpdateRental(dto, userId);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeleteRental(int id)
    {
        var  userClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userClaim == null)
            return Unauthorized("User not Authorized");
        var userId = int.Parse(userClaim);
        var res = await service.DeleteRental(id,  userId);
        return StatusCode((int)res.StatusCode, res);
    }
    
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetRental(int id)
    {
        var res = await service.GetRental(id);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetRentals([FromQuery]RentalFilter filter)
    {
        var res =  await service.GetRentals(filter);
        return StatusCode((int)res.StatusCode, res);
    }
    
    [HttpGet("me")]
    public async Task<IActionResult> GetRentalByUserId(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var userClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (userClaim == null)
            return Unauthorized("User not Authorized");
        
        var userId = int.Parse(userClaim);
        
        var res = await service.GetRentalByUserId(userId, pageNumber, pageSize);
        return StatusCode((int)res.StatusCode, res);
    }
}