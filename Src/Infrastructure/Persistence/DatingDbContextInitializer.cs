using Core.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Persistence
{
    public class DatingDbContextInitializer
    {
        public static void Initialize(DatingDbContext context, UserManager<User> user, RoleManager<Role> roleManager)
        {
            var initializer = new DatingDbContextInitializer();
            initializer.SeedEverything(context, user, roleManager);
        }

        private void SeedEverything(DatingDbContext context, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            context.Database.EnsureCreated();

            if (userManager.Users.Any())
            {
                return; // Db has been seeded
            }

            SeedUsers(userManager, roleManager);         
        }

        private void SeedUsers(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                var roles = new List<Role>
                {
                    new Role { Name="Admin"},
                    new Role { Name="Member"},               
                    new Role { Name="Moderator"}
                };

                foreach (var role in roles)
                {
                    roleManager.CreateAsync(role).Wait();
                }
            }

            var user = new User
            {
                Active = true,
                UserName = "userTest",
                City = "Lisbon",
                Country = "Portugal",
                DateOfBirth = DateTime.Now.AddYears(-30)                
            };

            userManager.CreateAsync(user, "12345678").Wait();
            userManager.AddToRoleAsync(user, "Member").Wait();
        }
    }
}
