using System.Text;
using Booking.Application.Interfaces.Repository;
using Booking.Application.Interfaces.Services;
using Booking.Application.Services;
using Booking.Application.Settings;
using Booking.Infrastructure.Data;
using Booking.Infrastructure.Repositories;
using Booking.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Booking.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var configuration = builder.Configuration;

        // ================= DATABASE =================

        builder.Services.AddDbContext<ApplicationDbContext>(
            options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString(
                        "SqlServerConnection"));
            });

        // ================= JWT SETTINGS =================

        var jwtSettings =
            configuration
                .GetSection("Jwt")
                .Get<JwtSettings>()
            ?? throw new Exception(
                "JWT settings not configured.");

        builder.Services.AddSingleton(jwtSettings);

        // ================= AUTHENTICATION =================

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,

                        ValidateAudience = true,

                        ValidateLifetime = true,

                        ValidateIssuerSigningKey = true,

                        ValidIssuer =
                            jwtSettings.Issuer,

                        ValidAudience =
                            jwtSettings.Audience,

                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(
                                    jwtSettings.Key)),

                        ClockSkew =
                            TimeSpan.Zero
                    };
            });

        // ================= AUTHORIZATION =================

        builder.Services.AddAuthorization();

        // ================= CONTROLLERS =================

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Description = "Enter JWT token"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        // ================= REPOSITORIES =================

        builder.Services.AddScoped<
            IUserRepository,
            UserRepository>();

        // ================= SERVICES =================

        builder.Services.AddScoped<
            IAuthService,
            AuthService>();

        builder.Services.AddScoped<
            IJwtService,
            JwtService>();
        var app = builder.Build();

        // ================= SWAGGER =================

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();

            app.UseSwaggerUI();
        }

        // ================= MIDDLEWARE =================

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
