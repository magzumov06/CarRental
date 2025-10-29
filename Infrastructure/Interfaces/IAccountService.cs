using Domain.DTOs.Account;
using Domain.Responces;

namespace Infrastructure.Interfaces;

public interface IAccountService
{
    Task<Responce<string>> RegisterAsync(Register register);
    Task<Responce<string>> LoginAsync(Login login);
}