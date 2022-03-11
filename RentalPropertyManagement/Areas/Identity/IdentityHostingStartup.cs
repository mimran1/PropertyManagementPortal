using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentalPropertyManagement.Data;

[assembly: HostingStartup(typeof(RentalPropertyManagement.Areas.Identity.IdentityHostingStartup))]
namespace RentalPropertyManagement.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<RentalPropertyManagementContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("RentalPropertyManagementContextConnection")));

                services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<RentalPropertyManagementContext>();
            });
        }
    }
}