global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;

global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using AutoMapper;

global using todolistasp.Services.ProductService;
global using todolistasp.Services.BaseService;
global using todolistasp.Services.UserService;
global using todolistasp.Services.OrderService;
global using todolistasp.Services.OrderItemService;
global using todolistasp.Services.CategoryService;
global using todolistasp.Dto;
global using todolistasp.Helpers;
global using todolistasp.Models;
global using todolistasp.Data;
global using todolistasp.Repository.BaseRepository;
global using todolistasp.Repository.ProductRepository;
global using todolistasp.Repository.UserRepository;
global using todolistasp.Repository.CategoryRepository;
global using todolistasp.Repository.OrderRepository;
global using todolistasp.Repository.ReviewRepository;
global using todolistasp.Repository.OrderItemRepository;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using todolistasp.Services.AuthService;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using todolistasp.Services.ReviewService;
using todolistasp.Milddlewares;
using todolistasp.Authorization;
using Microsoft.AspNetCore.Authorization;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        if (builder.Environment.IsDevelopment())
        {
            builder.WebHost.UseKestrel(options =>
            {
                options.ListenLocalhost(5000);
                // Set HTTPS port
                options.ListenLocalhost(
                    5001,
                    listenOptions =>
                    {
                        listenOptions.UseHttps();
                    }
                );
            });
        }

        builder.Services.Configure<RouteOptions>(options =>
        {
            options.LowercaseUrls = true;
        });

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition(
                "oauth2",
                new OpenApiSecurityScheme
                {
                    Description =
                        """Standard authorization using Bearer scheme, Example: "bearer {token}" """,
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                }
            );
            /*             c.AddSecurityRequirement(new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecurityScheme{
                                    Reference = new OpenApiReference{
                                        Id = "Bearer", //The name of the previously defined security scheme.
                                        Type = ReferenceType.SecurityScheme
                                    }
                                },new List<string>()
                            }
                        }); */
            c.OperationFilter<SecurityRequirementsOperationFilter>(); //does not work on "Bearer" authorization scheme
        });

        // Add service scope
        builder.Services
            .AddScoped<IProductService, ProductService>()
            .AddScoped<IUserService, UserService>()
            .AddScoped<IOrderService, OrderService>()
            .AddScoped<IOrderItemService, OrderItemService>()
            .AddScoped<IReviewService, ReviewService>()
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<ICategoryService, CategoryService>()
            .AddScoped<IProductRepository, ProductRepository>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IReviewRepository, ReviewRepository>()
            .AddScoped<IOrderRepository, OrderRepository>()
            .AddScoped<ICategoryRepository, CategoryRepository>()
            .AddScoped<IOrderItemRepository, OrderItemRepository>();

        builder.Services.AddSingleton<IAuthorizationHandler, ProductDeleteRequirementHandler>();
        builder.Services.AddSingleton<IAuthorizationHandler, UserUpdateRequirementHandler>();

        //builder.Services.AddTransient<LoggingMiddleware>();
        builder.Services.AddTransient<ErrorHandlerMiddleware>();

        // Add auto mapper
        builder.Services.AddAutoMapper(typeof(Program).Assembly);

        //Add DatabaseContext
        builder.Services.AddDbContext<DatabaseContext>();

        //http accessor
        builder.Services.AddHttpContextAccessor();

        // Add authentication service
        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(
                            builder.Configuration.GetSection("AppSettings:Token").Value!
                        )
                    )
                };
            });

        // Add authorization service
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnlyPolicy", policy => policy.RequireRole("Admin"));
            options.AddPolicy("SellerOnlyPolicy", policy => policy.RequireRole("Seller"));
            options.AddPolicy(
                "ProductDeletePolicy",
                policy => policy.AddRequirements(new ProductDeleteRequirement())
            );
            options.AddPolicy(
                "UserUpdatePolicy",
                policy => policy.AddRequirements(new UserUpdateRequirement())
            );
        });

        var app = builder.Build();

        app.UseHttpsRedirection();
        // Configure the HTTP request pipeline.S
        /*         if (app.Environment.IsDevelopment())
                { */
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend API");
            c.RoutePrefix = string.Empty;
        });
        /*   } */

        // app.UseMiddleware<LoggingMiddleware>();

        app.UseErrorHandler();

        app.UseLogging();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}