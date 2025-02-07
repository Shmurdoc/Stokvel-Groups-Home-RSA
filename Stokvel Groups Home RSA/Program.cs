using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stokvel_Groups_Home_RSA.Interface.IServices.IMessageServices;
using Stokvel_Groups_Home_RSA.Data;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Interface.IServices.IAccountProfileServices;
using Stokvel_Groups_Home_RSA.Interface.IServices.IAccountRequestService;
using Stokvel_Groups_Home_RSA.Interface.IServices.IDepositRequestService;
using Stokvel_Groups_Home_RSA.Interface.IServices.IGroupServices;
using Stokvel_Groups_Home_RSA.Interface.IServices.IPreDepositRequestService;
using Stokvel_Groups_Home_RSA.Interface.IServices.IWalletRepositoryService;
using Stokvel_Groups_Home_RSA.Repositories;
using Stokvel_Groups_Home_RSA.Services.AccountProfileService;
using Stokvel_Groups_Home_RSA.Services.AccountRequestService;
using Stokvel_Groups_Home_RSA.Services.DepositRequestService.DepositService;
using Stokvel_Groups_Home_RSA.Services.DepositRequestService.DepositSet;
using Stokvel_Groups_Home_RSA.Services.GroupServices;
using Stokvel_Groups_Home_RSA.Services.PreDepositRequestService.PreDepositInfo;
using Stokvel_Groups_Home_RSA.Services.WalletRequestService.Wallet;
using Stokvel_Groups_Home_RSA.Hubs;
using Stokvel_Groups_Home_RSA.Interface.IServices.IWithdrawServices;
using Stokvel_Groups_Home_RSA.Services.WithdrawServices;
using Stokvel_Groups_Home_RSA.Interface.IServices.IHomeService;
using Stokvel_Groups_Home_RSA.Services.DepositRequestService.DepoChildClass;
using Stokvel_Groups_Home_RSA.Services.HomeService;
using Stokvel_Groups_Home_RSA.Areas.Identity.Pages.Admin;
using Stokvel_Groups_Home_RSA.Areas.Identity.Pages.Role;

namespace Stokvel_Groups_Home_RSA.Controllers;

public class Program
{

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                   .EnableSensitiveDataLogging());

        // Identity setup
        builder.Services.AddDefaultIdentity<IdentityUser>().AddRoles<IdentityRole>()
                     .AddEntityFrameworkStores<ApplicationDbContext>()
                     .AddDefaultTokenProviders();


        builder.Services.AddSignalR();

        builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

        //services

        // Registering application services
        builder.Services.AddTransient<IDepositService, DepositService>();

        builder.Services.AddTransient<IDepositRequestServices, DepositRequest>();
        builder.Services.AddTransient<IDepositRequestServices, PreDepositDepositRequest>();
        builder.Services.AddTransient<IDepositRequestServices, WalletDepositRequest>();
        builder.Services.AddTransient<IGroupRequestServices, GroupRequestServices>();
        builder.Services.AddTransient<IAccountProfileRequestServices, AccountProfileServices>();
        builder.Services.AddTransient<IWithdrawServices, WithdrawServices>();
        builder.Services.AddTransient<IAccountRequestServices, AccountRequestServices>();
        builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
        builder.Services.AddTransient<IHomeRequestService, HomeRequestService>();
        builder.Services.AddTransient<IAccountProfileRequestServices, AccountProfileServices>();
        builder.Services.AddTransient<IGroupRequestServices, GroupRequestServices>();
        builder.Services.AddTransient<IPreDepositRequestServices, PreDepositInfo>();
        builder.Services.AddTransient<IWalletRequestServices, WalletInfo>();
        builder.Services.AddTransient<IWithdrawRequestService, WithdrawRequestService>();
        builder.Services.AddTransient<IWithdrawServices, WithdrawServices>();
        builder.Services.AddTransient<IDepositSet, DepositSet>();

        //Other dependencies
        builder.Services.AddTransient<RoleSeeder>();
        builder.Services.AddTransient<AdminSeeder>();
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
        builder.Services.AddMvc();




        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
            app.UseMvcWithDefaultRoute();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}"
            );
        app.MapHub<ChatHub>("ChatHub");
        app.MapRazorPages();

        try
        {
            using (var scope = app.Services.CreateScope())
            {
                var roleSeeder = scope.ServiceProvider.GetRequiredService<RoleSeeder>();
                var adminSeeder = scope.ServiceProvider.GetRequiredService<AdminSeeder>();

                await roleSeeder.CreateRolesAsync();
                await adminSeeder.CreateAdminUserAsync();
            }
        }
        catch (Exception ex)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred during application startup.");
        }

        app.Run();
    }
}
