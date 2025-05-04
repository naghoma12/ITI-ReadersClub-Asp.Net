using Microsoft.AspNetCore.Identity;
using ReadersClubCore.Data;
using ReadersClubCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ReadersClubCore.DataSeed
{
    public static class AdminDataSeed
    {
        public static async Task SeedAdminAccount(ReadersClubContext context,UserManager<ApplicationUser> userManager,RoleManager<ApplicationRole> roleManager)
        {
           
            if(context.Users.Any() && context.Roles.Any())
            {
                return;
            }
            using var transaction = context.Database.BeginTransaction();
            {
                try
                {
                    var adminUser = new ApplicationUser()
                    {
                        UserName = "admin",
                        Name = "admin",
                        Image = ""
                    };

                    var roles = new List<ApplicationRole>()
                    {
                       new ApplicationRole(){ Name = "admin" },
                       new ApplicationRole(){ Name = "author" },
                       new ApplicationRole(){ Name = "reader"}
                    };

                    var result = await userManager.CreateAsync(adminUser, "AdminUser@12");
                    foreach (var role in roles)
                    {
                        var roleResult = await roleManager.CreateAsync(role);
                    }
                    var addToRoleResult = await userManager.AddToRoleAsync(adminUser, roles.FirstOrDefault(x => x.Name == "admin").Name);
                    transaction.Commit();
                } catch (Exception ex) 
                {
                    transaction.Rollback();
                 }
               
            } 
        }
    }
}
