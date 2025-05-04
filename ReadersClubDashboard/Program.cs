using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using ReadersClubCore.Data;
using ReadersClubCore.DataSeed;
using ReadersClubCore.Models;
using ReadersClubDashboard.Helper;
using ReadersClubDashboard.Service;
using ReadersClubDashboard.Sevice;
using System.Reflection;

namespace ReadersClubDashboard
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddScoped<CategoryService>();
            builder.Services.AddScoped<StoryService>();
            builder.Services.AddScoped<ChannelService>();
            builder.Services.AddScoped<ReviewService>();

            // Add services to the container.
            #region Services

            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ReadersClubContext>(
                options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("default"))
            );
            //Auto Mapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            #region Identity
            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ReadersClubContext>()
                .AddDefaultTokenProviders();

            #endregion

            #region Cookies
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
          .AddCookie(options =>
          {
              options.LoginPath = "/Account/Login/";
              options.LogoutPath = "/Account/Logout/";
          });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });
            #endregion
            #endregion
            var app = builder.Build();

            #region AppSeedingConfig    
            try
            {
                var scope = app.Services.CreateScope();
                {
                    var services = scope.ServiceProvider;
                    var context = services.GetRequiredService<ReadersClubContext>();
                   await context.Database.MigrateAsync();
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
                    await AdminDataSeed.SeedAdminAccount(context, userManager, roleManager);
                }
            }
            catch
            {
                Console.WriteLine("Error in seeding data"); 
            }
            
            #endregion

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
           
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
