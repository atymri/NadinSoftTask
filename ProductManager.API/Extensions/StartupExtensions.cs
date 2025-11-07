using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
            builder.Services.AddTransient<IJwtService, JwtService>();

            var mongoConfig = builder.Configuration.GetSection("MongoDbSettings");
            var mongoConnection = mongoConfig["ConnectionString"]
                                  ?? throw new KeyNotFoundException("Mongo connection string is missing!!!!!");
            var mongoDatabase = mongoConfig["DatabaseName"]
                                ?? throw new KeyNotFoundException("Mongo database name is missing!!!!!");

            builder.Services.AddSingleton(sp =>
            {
                var context = new MongoDbContext(mongoConnection, mongoDatabase);
                context.EnsureIndexes(); 
                return context;
            });


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

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add(new ProducesAttribute("application/json"));
                options.Filters.Add(new ConsumesAttribute("application/json"));
            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                };
            });

            var connectionString = builder.Configuration.GetConnectionString("Default") ??
                                   throw new KeyNotFoundException("connection string is missing!!!!!");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "توکن را به شکل زیر وارد کنید: \n {your_token}"
                });

                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

        }
    }
}
