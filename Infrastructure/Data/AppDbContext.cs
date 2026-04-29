using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data;

using Core.Entities;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<ShortUrl> ShortUrls => Set<ShortUrl>();
    public DbSet<AlgorithmInfo> Infos => Set<AlgorithmInfo>();
}
