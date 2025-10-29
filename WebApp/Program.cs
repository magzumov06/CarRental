using Domain.DTOs.EmailDto;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Data.Seeder;
using Infrastructure.ExtensionMethod;
using Infrastructure.FileStorage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterDataContext(builder.Configuration);

builder.Services.RegisterIdentity();

builder.Services.RegisterSwagger();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IFileStorage>(sp => new FileStorage(builder.Environment.ContentRootPath));


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.RegisterServices();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var data = services.GetRequiredService<DataContext>();
        await data.Database.MigrateAsync();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
        await Seed.SeedRole(roleManager);
        var userManager = services.GetRequiredService<UserManager<User>>();
        await Seed.SeedAdmin(userManager, roleManager);
    }
    catch (Exception ex)
    {
        //
    }
}

app.Run();