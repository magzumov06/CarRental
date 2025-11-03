using Domain.DTOs.RentalDto;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controller;


[ApiController]
[Route("api/[controller]")]
public class RentalController(IRentalService service) :  ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateRental([FromBody] CreateRentalDto dto)
    {
        var res = await service.CreateRental(dto);
        return StatusCode((int)res.StatusCode, res);
    }


    [HttpPut]
    public async Task<IActionResult> UpdateRental(UpdateRentalDto dto)
    {
        var res = await service.UpdateRental(dto);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteRental(int id)
    {
        var res = await service.DeleteRental(id);
        return StatusCode((int)res.StatusCode, res);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRental(int id)
    {
        var res = await service.GetRental(id);
        return StatusCode((int)res.StatusCode, res);
    }
}