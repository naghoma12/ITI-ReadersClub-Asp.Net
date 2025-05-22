using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using ReadersClubApi.Helper;
using ReadersClubApi.Service;
using ReadersClubApi.Services;
using ReadersClubCore.Data;
using ReadersClubCore.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.StaticFiles;

namespace ReadersClubApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<ReadersClubContext>(
                options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("default"))
            );

            builder.Services.AddScoped<TokenConfiguration>();
            builder.Services.AddScoped<ProfileService>();
            builder.Services.AddScoped<ReviewService>();
            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(
                options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredLength = 8;
                })
                .AddEntityFrameworkStores<ReadersClubContext>()
                .AddDefaultTokenProviders();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).
                 AddJwtBearer(options =>
                 options.TokenValidationParameters = new TokenValidationParameters()
                 {
                     ValidateAudience = true,
                     ValidAudience = builder.Configuration["JWT:Audience"],
                     ValidateIssuer = true,
                     ValidIssuer = builder.Configuration["JWT:Issuer"],
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"])),
                     ValidateLifetime = true,
                     ClockSkew = TimeSpan.FromDays(double.Parse(builder.Configuration["JWT:DurationExpire"]))
                 });
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
            builder.Services.AddScoped<IMailService, MailService>();
            builder.Services.AddScoped<StoryService>();
            //add channel service
            builder.Services.AddScoped<IChannelService, ChannelService>();


            // ✅ إعداد CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowDashboard", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
            builder.Services.AddSwaggerGen(swagger =>
            {

                // To Enable authorization using Swagger (JWT)    
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });

                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
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

            var app = builder.Build();
            var env = app.Environment;
            // Configure the HTTP request pipeline.
           // if (app.Environment.IsDevelopment())
            //{
                app.UseSwagger();
                app.UseSwaggerUI();
            //}

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
        Path.Combine(env.WebRootPath, "Uploads")), 
                RequestPath = "/Uploads",
       
            });

            app.UseCors("AllowDashboard");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
