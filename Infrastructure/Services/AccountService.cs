using System.Net;
using System.Security.Claims;
using Domain.DTOs.Account;
using Domain.DTOs.EmailDto;
using Domain.Entities;
using Domain.Responces;
using Infrastructure.Data;
using Infrastructure.FileStorage;
using Infrastructure.Helpers;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Infrastructure.Services;

public class AccountService(
    UserManager<User> userManager,
    DataContext context,
    IConfiguration  configuration,
    IEmailSender emailSender,
    IFileStorage file
    ) :  IAccountService
{
    public async Task<Responce<string>> RegisterAsync(Register register)
    {
        try
        {
            Log.Information("Registering account");
            var existingUser = await userManager.FindByNameAsync(register.UserName);
            if (existingUser != null)
                return new Responce<string>(HttpStatusCode.BadRequest, "User already exists");
            var existingEmail = await userManager.FindByEmailAsync(register.Email);
            if (existingEmail != null)
                return new Responce<string>(HttpStatusCode.BadRequest, "Email already exists");
            var user = new User
            {
                FullName = register.FullName,
                UserName = register.UserName, 
                Email = register.Email,
                CreatedDate = DateTime.UtcNow,
            };
            if (register.ProfilePicture != null)
            {
                user.ProfilePicture = await file.UploadFile(register.ProfilePicture, "UserAvatar");
            }
            var password = PasswordUtil.GenerateRandomPassword();
            var result = await userManager.CreateAsync(user, password);
            await userManager.AddToRoleAsync(user, "User");
            if (!result.Succeeded)
                return new Responce<string>(HttpStatusCode.BadRequest, "Something went wrong");
            await emailSender.SendEmailAsync(new SendEmail
            {
                To = user.Email, Subject = "Welcome to the CarRental", Body =
                    $"<p>Салом {user.FullName}!</p><br>Логини шумо {user.UserName}<br>Пароли шумо:{password}</p>"
            });
            return new Responce<string>("Customer created and email sent");
        }
        catch (Exception e)
        {
            Log.Error("Error in creating account");
            return new Responce<string>(HttpStatusCode.InternalServerError, e.Message);
        }    
    }
    public async Task<Responce<string>> LoginAsync(Login login)
    {
        try
        {
            var user = await userManager.FindByNameAsync(login.UserName);
            if (user == null)
                return new Responce<string>(HttpStatusCode.NotFound, "Номи корбар ё рамз нодуруст аст");
            var isPasswordValid = await userManager.CheckPasswordAsync(user, login.Password);
            if (!isPasswordValid)
                return new Responce<string>(HttpStatusCode.BadRequest, "Номи корбар ё рамз нодуруст аст");
            var token = await GenerateJwtTokenHelper.GenerateJwtToken(user, userManager, configuration);
            return new Responce<string>(token) { Message = "Воридшавӣ бо муваффақият анҷом ёфт" };
        }
        catch (Exception e)
        {
            return new Responce<string>(HttpStatusCode.InternalServerError, e.Message);
        }    }

    public async Task<Responce<string>> ChangePassword(ChangePassword changePassword)
    {
        try
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == changePassword.UserId);
            if(user == null) return new Responce<string>(HttpStatusCode.NotFound, "User not found");
            
            var res = await userManager.ChangePasswordAsync(user, changePassword.OldPassword, changePassword.NewPassword);
            
            if(!res.Succeeded)
                return new Responce<string>(HttpStatusCode.BadRequest, "Old password went wrong");
            return new Responce<string>(HttpStatusCode.OK,"Password changed");
        }
        catch (Exception e)
        {
            return new Responce<string>(HttpStatusCode.InternalServerError,e.Message);
        }
    }
}