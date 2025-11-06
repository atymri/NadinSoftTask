using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductManager.Core.Domain.Entities;
using ProductManager.Core.Domain.RepositoryContracts;
using ProductManager.Core.DTOs.MapperProfiles;
using ProductManager.Core.Exceptions;
using ProductManager.Core.ServiceContracts;
using ProductManager.Core.Services;
using ProductManager.Infrastructure.DatabaseContext;
using ProductManager.Infrastructure.Repositories;

namespace ProductManager.API.Extensions
{
    public static class StartupExtensions
    {
        public static void ConfigureServices(this WebApplicationBuilder builder)
        { 
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductGetterService, ProductGetterService>();
            builder.Services.AddScoped<IProductAdderService, ProductAdderService>();
            builder.Services.AddScoped<IProductUpdaterService, ProductUpdaterService>();
            builder.Services.AddScoped<IProductDeleterService, ProductDeleterService>();

            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddMaps(typeof(ProductMapperProfile).Assembly);
                cfg.AddMaps(typeof(UserMapperProfile).Assembly);
            });

            builder.Services.AddIdentity<User, Role>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequiredUniqueChars = 3;
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddErrorDescriber<PersianIdentityErrorDescriber>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
            });

            builder.Services.AddControllers();

            var connectionString = builder.Configuration.GetConnectionString("Default") ??
                                   throw new KeyNotFoundException("connection string is missing!!!!!");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
        }
    }
}
