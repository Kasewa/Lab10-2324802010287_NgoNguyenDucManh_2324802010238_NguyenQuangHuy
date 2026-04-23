using ASC.Model.BaseTypes;
using ASC.Web.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace ASC.Web.Data
{
    public class IdentitySeed : IIdentitySeed
    {
        public async Task Seed(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<ApplicationSettings> options)
        {
            // Create roles if they don't exist
            var roles = options.Value.Roles.Split(new char[] { ',' });
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role.Trim()))
                {
                    await roleManager.CreateAsync(new IdentityRole { Name = role.Trim() });
                }
            }

            // Ensure Admin exists and is confirmed
            var admin = await userManager.FindByEmailAsync(options.Value.AdminEmail);
            if (admin == null)
            {
                var user = new IdentityUser
                {
                    UserName = options.Value.AdminName,
                    Email = options.Value.AdminEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, options.Value.AdminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddClaimAsync(user, new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", options.Value.AdminEmail));
                    await userManager.AddClaimAsync(user, new Claim("IsActive", "True"));
                    await userManager.AddToRoleAsync(user, Roles.Admin.ToString());
                }
            }
            else if (!admin.EmailConfirmed)
            {
                // Fix existing admin whose email was not confirmed
                admin.EmailConfirmed = true;
                await userManager.UpdateAsync(admin);
            }

            // Ensure Engineer exists and is confirmed
            var engineer = await userManager.FindByEmailAsync(options.Value.EngineerEmail);
            if (engineer == null)
            {
                var user = new IdentityUser
                {
                    UserName = options.Value.EngineerName,
                    Email = options.Value.EngineerEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, options.Value.EngineerPassword);
                if (result.Succeeded)
                {
                    await userManager.AddClaimAsync(user, new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", options.Value.EngineerEmail));
                    await userManager.AddClaimAsync(user, new Claim("IsActive", "True"));
                    await userManager.AddToRoleAsync(user, Roles.Engineer.ToString());
                }
            }
            else if (!engineer.EmailConfirmed)
            {
                engineer.EmailConfirmed = true;
                await userManager.UpdateAsync(engineer);
            }
        }
    }
}
