using Domain.DTOs.EmailDto;
using Infrastructure.Helpers;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class EmailSender(IOptions<EmailSettings> options):IEmailSender
{
    private readonly EmailSettings _settings = options.Value;
    public async Task SendEmailAsync(SendEmail dto)
    {
        await EmailHelper.SendEmail(dto, _settings);
    }
}