﻿using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext<User, IdentityRole<int>, int>(options)
{
    public DbSet<Car> Cars { get; set; }
    public DbSet<Rental> Rentals { get; set; }
}
