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
        return StatusCode((int)res.StatusCode, res);

    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]Login request)
    {
        var res = await service.LoginAsync(request);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpPut]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePassword request)
    {
        var res =  await service.ChangePassword(request);
        return StatusCode((int)res.StatusCode, res);
    }
}