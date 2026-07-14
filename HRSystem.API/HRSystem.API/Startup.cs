using System;
using System.IO;
using System.Text;
using HRSystem.Application.Employees.Queries;
using HRSystem.Application.Mappings;
using HRSystem.Infrastructure;
using HRSystem.Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace HRSystem.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var jwtOptions = Configuration.GetSection("JwtSetting").Get<JwtOptions>();

            services.AddControllers();
            services.AddLocalization();
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "DataProtectionKeys")));

            services.Configure<JwtOptions>(Configuration.GetSection("JwtSetting"));
            services.AddMediatR(typeof(GetEmployeesQuery).Assembly);
            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            services.AddInfrastructure(Configuration);

            // Enable CORS for Angular dev server - origins come from configuration
            var allowedOrigins = Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularDevClient", builder =>
                {
                    builder.WithOrigins(allowedOrigins ?? new string[0])
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
                    };
                });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "HR System API",
                    Version = "v1"
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter: Bearer {your JWT token}"
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
                        new string[] { }
                    }
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "HR System API v1");
                options.RoutePrefix = "swagger";
            });

            if (!string.IsNullOrWhiteSpace(Configuration["HttpsRedirection:HttpsPort"]))
            {
                app.UseHttpsRedirection();
            }

            var supportedCultures = new[] { "en", "ar" };
            app.UseRequestLocalization(options =>
            {
                options.SetDefaultCulture("en");
                options.AddSupportedCultures(supportedCultures);
                options.AddSupportedUICultures(supportedCultures);
            });

            app.UseRouting();

            // Use CORS policy for requests from Angular dev server
            app.UseCors("AllowAngularDevClient");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
