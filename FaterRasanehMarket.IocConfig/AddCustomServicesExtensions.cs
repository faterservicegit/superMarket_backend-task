using Coravel;
using FaterRasanehMarket.Data.Contracts;
using FaterRasanehMarket.Data.UnitOfWork;
using FaterRasanehMarket.Services;
using FaterRasanehMarket.Services.Contracts;
using FaterRasanehMarket.Services.Hubs;
using Microsoft.Extensions.DependencyInjection;

namespace FaterRasanehMarket.IocConfig
{
    public static class AddCustomServicesExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<ISmsSender, SmsSender>();
            services.AddTransient<RegisterDayVisits>();
            services.AddScoped<SellerHub>();
            services.AddScheduler();
            return services;
        }
    }
}
