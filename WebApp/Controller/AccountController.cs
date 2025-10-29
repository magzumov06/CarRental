using Domain.DTOs.Account;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controller;

[ApiController]
[Route("api/[controller]")]
public class AccountController(IAccountService service) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm] Register request)
    {
        var res = await service.RegisterAsync(request);
        if (res.StatusCode != 200)
        {
            return BadRequest(res);
        }
        return Ok(res);
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]Login request)
    {
        var res = await service.LoginAsync(request);
        if (res.StatusCode != 200)
        {
            return BadRequest(res);
        }
        return Ok(res);
    }
}