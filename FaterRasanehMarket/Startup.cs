using AutoMapper;
using Coravel;
using FaterRasanehMarket.Data;
using FaterRasanehMarket.IocConfig;
using FaterRasanehMarket.IocConfig.AutoMapper;
using FaterRasanehMarket.Services;
using FaterRasanehMarket.Services.Hubs;
using FaterRasanehMarket.ViewModels.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace FaterRasanehMarket
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
            services.AddControllersWithViews();
            services.Configure<SiteSettings>(Configuration.GetSection(nameof(SiteSettings)));
            services.ConfigureWritable<SiteSettings>(Configuration.GetSection("SiteSettings"));
            services.AddCustomIdentityServices();
            services.AddCustomServices();
            services.AddSignalR(opt=> {
                opt.ClientTimeoutInterval = TimeSpan.FromHours(24);
            });
            services.AddAutoMapper(c => c.AddProfile<MappingProfiles>(), typeof(Startup));
            services.AddDbContext<FaterRasanehMarketDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("SqlServer")));
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/Error404";
                    await next();
                }
            });
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCustomIdentityServices();
            app.UseAuthorization();
            var provider = app.ApplicationServices;
            provider.UseScheduler(scheduler =>
            {
                scheduler.Schedule<RegisterDayVisits>().DailyAt(23,59).Zoned(TimeZoneInfo.Local); //UTC Time
            });
                app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
                    );
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<SellerHub>("/Hubs/Seller");
            });
        }
    }
}
