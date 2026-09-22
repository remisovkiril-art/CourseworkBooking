using Booking.Application.Interfaces.Repository;
using Booking.Application.Interfaces.Services;
using Booking.Application.Mapping;
using Booking.Application.Services;
using Booking.Application.Settings;
using Booking.Infrastructure.Data;
using Booking.Infrastructure.Repositories;
using Booking.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Booking.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var configuration = builder.Configuration;

        builder.Services.AddDbContext<ApplicationDbContext>(
            options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString(
                        "SqlServerConnection"));
            });

        var jwtSettings =
            configuration
                .GetSection("Jwt")
                .Get<JwtSettings>()
            ?? throw new Exception("JWT settings not configured.");

        builder.Services.AddSingleton(jwtSettings);

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultSignInScheme =
                    CookieAuthenticationDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,

                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(
                                    configuration["Jwt:Key"]!)),

                        ValidateIssuer = true,
                        ValidIssuer = configuration["Jwt:Issuer"],

                        ValidateAudience = true,
                        ValidAudience = configuration["Jwt:Audience"],

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
            })
            .AddCookie(
                CookieAuthenticationDefaults.AuthenticationScheme,
                options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Cookie.SecurePolicy =
                        CookieSecurePolicy.SameAsRequest;
                })
            .AddGoogle(options =>
            {
                options.ClientId =
                    configuration[
                        "Authentication:Google:ClientId"]!;

                options.ClientSecret =
                    configuration[
                        "Authentication:Google:ClientSecret"]!;

                options.SignInScheme =
                    CookieAuthenticationDefaults.AuthenticationScheme;
            });

        builder.Services.AddAuthorization();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("ReactPolicy", policy =>
            {
                policy
                    .WithOrigins(
                        "http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT token"
                });

            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference =
                                new OpenApiReference
                                {
                                    Type =
                                        ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                        },
                        Array.Empty<string>()
                    }
                });
        });

        // ================= AutoMapper =================
        builder.Services.AddAutoMapper(
            _ => { }, typeof(UserProfile).Assembly, typeof(ReviewProfile).Assembly,
            typeof(HotelProfile).Assembly,
            typeof(RoomProfile).Assembly
            );


        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IHotelRepository, HotelRepository>();
        builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
        builder.Services.AddScoped<IBookingRepository, BookingRepository>();
        builder.Services.AddScoped<IRoomRepository, RoomRepository>();
        builder.Services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();

        builder.Services.AddScoped<IPaymentMethodService, PaymentMethodService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IRoomService, RoomService>();
        builder.Services.AddScoped<IJwtService, JwtService>();
        builder.Services.AddScoped<IHotelService, HotelService>();
        builder.Services.AddScoped<IReviewService, ReviewService>();
        builder.Services.AddScoped<IBookingService, BookingService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IImageService, ImageService>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseStaticFiles();

        app.UseCors("ReactPolicy");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
