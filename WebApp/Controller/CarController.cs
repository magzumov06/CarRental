using Domain.DTOs.CarDto;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controller;

[ApiController]
[Route("api/[controller]")]
public class CarController(ICarService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddCar([FromForm] AddCarDto dto)
    {
        var res =  await service.AddCar(dto);
        return Ok(res);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCar([FromForm] UpdateCarDto dto)
    {
        var res = await service.UpdateCar(dto);
        return Ok(res);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        var res = await service.DeleteCar(id);
        return Ok(res);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCar(int id)
    {
        var res = await service.GetCarById(id);
        return Ok(res);
    }
}