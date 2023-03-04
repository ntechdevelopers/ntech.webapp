using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ntech.WebApp.Data;
using Ntech.WebApp.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Ntech.WebApp.Permission;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace Ntech.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");

            // /*** Sql Server ***/
            // services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(connectionString)
            // );

            /*** My Sql ***/
            // services.AddDbContextPool<ApplicationDbContext>(options =>
            //     options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            // );

            /*** Postgres ***/
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseNpgsql(connectionString)
            );
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

            services.AddIdentity<IdentityUser, IdentityRole>()
                    .AddDefaultTokenProviders()
                    .AddDefaultUI()
                    .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddTransient<IEmailSender, EmailSender>();

            services.AddDistributedMemoryCache();
            services.AddHttpContextAccessor();
            services.AddControllersWithViews();

            services.AddDataProtection();
            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                // You might want to only set the application cookies over a secure connection:
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.HttpOnly = true;
                // Make the session cookie essential
                options.Cookie.IsEssential = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Dashboard}/{action=Index}/{id?}");
            });
        }
    }
}
