using FaterRasanehMarket.Common;
using FaterRasanehMarket.Data;
using FaterRasanehMarket.Entities.identity;
using FaterRasanehMarket.Services.Contracts;
using FaterRasanehMarket.ViewModels.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace FaterRasanehMarket.Services.Identity
{

    public class IdentityDbInitializer : IIdentityDbInitializer
    {
        private readonly IOptionsSnapshot<SiteSettings> _adminUserSeedOptions;
        private readonly IApplicationUserManager _applicationUserManager;
        private readonly ILogger<IdentityDbInitializer> _logger;
        private readonly IApplicationRoleManager _roleManager;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly string[] roles = { "مدیریت", "فروشنده", "کاربر" };
        public IdentityDbInitializer(
            IApplicationUserManager applicationUserManager,
            IServiceScopeFactory scopeFactory,
            IApplicationRoleManager roleManager,
            IOptionsSnapshot<SiteSettings> adminUserSeedOptions,
            ILogger<IdentityDbInitializer> logger
            )
        {
            _applicationUserManager = applicationUserManager;
            _scopeFactory = scopeFactory;
            _roleManager = roleManager;
            _adminUserSeedOptions = adminUserSeedOptions;
            _logger = logger;
        }

        /// <summary>
        /// Applies any pending migrations for the context to the database.
        /// Will create the database if it does not already exist.
        /// </summary>
        public void Initialize()
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<FaterRasanehMarketDBContext>())
                {
                    var result = (context.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists();
                    if (result == false)
                    {
                        (context.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Create();
                    }
                    try
                    {
                        (context.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).CreateTables();
                    }
                    catch { }
                }
            }
        }
        /// <summary>
        /// Adds some default values to the IdentityDb
        /// </summary>
        public void SeedData()
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                var identityDbSeedData = serviceScope.ServiceProvider.GetService<IIdentityDbInitializer>();
                var result = identityDbSeedData.SeedDatabaseWithAdminUserAsync().Result;
                if (result == IdentityResult.Failed())
                {
                    throw new InvalidOperationException(result.DumpErrors());
                }
            }
        }
        public async Task<IdentityResult> SeedDatabaseWithAdminUserAsync()
        {
            var adminUserSeed = _adminUserSeedOptions.Value.AdminUserSeed;

            var name = adminUserSeed.Username;
            var password = adminUserSeed.Password;
            var roleName = adminUserSeed.RoleName;
            var firstName = adminUserSeed.FirstName;
            var lastName = adminUserSeed.LastName;
            var email = adminUserSeed.Email;

            var thisMethodName = nameof(SeedDatabaseWithAdminUserAsync);

            var adminUser = await _applicationUserManager.FindByNameAsync(name);
            if (adminUser != null)
            {
                _logger.LogInformation($"{thisMethodName}: adminUser already exists.");
                return IdentityResult.Success;
            }

            //Create the `Admin` Role if it does not exist
            foreach (var role in roles)
            {
                var Role = await _roleManager.FindByNameAsync(role);
                if (Role == null)
                {
                    Role = new Role(role);
                    var adminRoleResult = await _roleManager.CreateAsync(Role);
                    if (adminRoleResult == IdentityResult.Failed())
                    {
                        _logger.LogError($"{thisMethodName}: adminRole CreateAsync failed. {adminRoleResult.DumpErrors()}");
                        return IdentityResult.Failed();
                    }
                }
                else
                {
                    _logger.LogInformation($"{thisMethodName}: adminRole already exists.");
                }
            }
            adminUser = new User
            {
                UserName = name,
                RegisterDateTime = DateTime.Now,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
            };
            var adminUserResult = await _applicationUserManager.CreateAsync(adminUser, password);
            if (adminUserResult == IdentityResult.Failed())
            {
                _logger.LogError($"{thisMethodName}: adminUser CreateAsync failed. {adminUserResult.DumpErrors()}");
                return IdentityResult.Failed();
            }

            var setLockoutResult = await _applicationUserManager.SetLockoutEnabledAsync(adminUser, enabled: false);
            if (setLockoutResult == IdentityResult.Failed())
            {
                _logger.LogError($"{thisMethodName}: adminUser SetLockoutEnabledAsync failed.");
                return IdentityResult.Failed();
            }

            var addToRoleResult = await _applicationUserManager.AddToRoleAsync(adminUser, roles[0]);
            if (addToRoleResult == IdentityResult.Failed())
            {
                _logger.LogError($"{thisMethodName}: adminUser AddToRoleAsync failed. {addToRoleResult.DumpErrors()}");
                return IdentityResult.Failed();
            }
            return IdentityResult.Success;
        }
    }
}