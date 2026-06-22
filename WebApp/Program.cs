using Domain.DTOs.EmailDto;
using Domain.Entities;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure.Data;
using Infrastructure.Data.Seeder;
using Infrastructure.ExtensionMethod;
using Infrastructure.FileStorage;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Hangfire.Dashboard;

var builder = WebApplication.CreateBuilder(args);

#region 🔥 SERILOG
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();

builder.Host.UseSerilog();
#endregion

#region 🗄️ DATABASE
builder.Services.RegisterDataContext(builder.Configuration);
#endregion

#region 🔐 IDENTITY + JWT
builder.Services.RegisterIdentity();
builder.Services.RegisterJwt(builder.Configuration);
#endregion

#region 📚 SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.RegisterSwagger();
#endregion

#region ⚙️ SERVICES
builder.Services.RegisterServices();

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddScoped<IFileStorage>(sp =>
    new FileStorage(builder.Environment.ContentRootPath));
#endregion

#region 🔥 HANGFIRE
builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfireServer();
#endregion

builder.Services.AddControllers();

var app = builder.Build();

#region 🌐 MIDDLEWARE ORDER
app.UseHttpsRedirection();

app.UseCors(policy =>
{
    policy.AllowAnyOrigin()
          .AllowAnyHeader()
          .AllowAnyMethod();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
#endregion

#region 📚 SWAGGER (DEV ONLY)

    app.UseSwagger();
    app.UseSwaggerUI();

#endregion

#region 🔥 HANGFIRE DASHBOARD (SECURED)
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});
#endregion

// #region ⏰ BACKGROUND JOBS
// RecurringJob.AddOrUpdate<IRentalService>(
//     "complete-expired-rentals",
//     x => x.MarkExpiredRentalsAsCompleted(),
//     Cron.Daily(8)
// );
// #endregion

#region 🗄️ DB MIGRATION + SEED
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
        Log.Error(ex, "Database migration or seeding failed");
    }
}
#endregion

app.Run();

#region 🔐 HANGFIRE AUTH FILTER
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        return httpContext.User.Identity?.IsAuthenticated == true
               && httpContext.User.IsInRole("Admin");
    }
}
#endregion