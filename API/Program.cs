using API.Data;
using API.Helpers;
using API.Services.ORM;
using API.Services.ORM.Master_Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using System.Text;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Define the policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyMvcAppPolicy", policy =>
                {
                    policy.WithOrigins("http://localhost:8090", "https://procurement-app.my.id", "http://localhost:3000") // Allow your MVC app
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

            // Add services to the container.
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
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key)
                };
            });
            builder.Services.AddAuthorization();
            builder.Services.AddControllers();
            builder.Services.AddDbContext<DataContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DockerPostgresConnection"));
            });
            builder.Services.AddScoped<BranchService>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<ProcurementDepartmentService>();
            builder.Services.AddScoped<QueryHelper>();
            builder.Services.AddScoped<ModuleService>();
            builder.Services.AddScoped<StatusService>();
            builder.Services.AddScoped<NonCommercialService>();
            builder.Services.AddScoped<RoleService>();
            builder.Services.AddScoped<ContractTypeService>();
            builder.Services.AddScoped<VendorService>();
            builder.Services.AddScoped<UserProcDeptService>();
            builder.Services.AddScoped<GeneralLedgerService>();
            builder.Services.AddScoped<MaterialAnaplanService>();


            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("MyMvcAppPolicy");
            app.UseAuthorization();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}
