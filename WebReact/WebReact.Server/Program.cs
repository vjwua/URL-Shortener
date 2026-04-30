using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddOpenApi();

        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        //builder.Services.AddCors(options =>
        //{
        //    options.AddPolicy("ReactApp", policy =>
        //    {
        //        policy.WithOrigins("http://localhost:5173") //Frontend
        //              .AllowAnyHeader()
        //              .AllowAnyMethod();
        //    });
        //});

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var adminEmail = builder.Configuration["Identity:DefaultAdmin:Email"] ?? "admin@shurl.com";
            var adminPassword = builder.Configuration["Identity:DefaultAdmin:Password"] ?? "Admin123!";
            await DataSeeder.SeedAsync(userManager, roleManager, adminEmail, adminPassword);
        }

        app.UseDefaultFiles();
        app.MapStaticAssets();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.MapFallbackToFile("/index.html");

        app.Run();
    }
}