using Domain.DTOs.EmailDto;

namespace Infrastructure.Interfaces;

public interface IEmailSender
{
    Task SendEmailAsync(SendEmail dto);
}