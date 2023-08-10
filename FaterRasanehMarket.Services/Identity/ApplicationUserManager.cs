using FaterRasanehMarket.Common;
using FaterRasanehMarket.Entities.identity;
using FaterRasanehMarket.Services.Contracts;
using FaterRasanehMarket.ViewModels.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FaterRasanehMarket.Services.Identity
{
    public class ApplicationUserManager : UserManager<User>, IApplicationUserManager
    {
        private readonly ApplicationIdentityErrorDescriber _errors;
        private readonly ILookupNormalizer _keyNormalizer;
        private readonly ILogger<ApplicationUserManager> _logger;
        private readonly IOptions<IdentityOptions> _options;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IEnumerable<IPasswordValidator<User>> _passwordValidators;
        private readonly IServiceProvider _services;
        private readonly IUserStore<User> _userStore;
        private readonly IEnumerable<IUserValidator<User>> _userValidators;
        public ApplicationUserManager(
            ApplicationIdentityErrorDescriber errors,
            ILookupNormalizer keyNormalizer,
            ILogger<ApplicationUserManager> logger,
            IOptions<IdentityOptions> options,
            IPasswordHasher<User> passwordHasher,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            IServiceProvider services,
            IUserStore<User> userStore,
            IEnumerable<IUserValidator<User>> userValidators)
            : base(userStore, options, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _userStore = userStore;
            _errors = errors;
            _logger = logger;
            _services = services;
            _passwordHasher = passwordHasher;
            _userValidators = userValidators;
            _options = options;
            _keyNormalizer = keyNormalizer;
            _passwordValidators = passwordValidators;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await Users.ToListAsync();
        }
        public async Task<List<UsersViewModel>> GetAllUsersWithRolesAsync()
        {
            return await Users.Select(user => new UsersViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                CreditCardNumber= user.CreditCardNumber,
                RegisterDateTime = user.RegisterDateTime,
                PhoneNumberConfirmed=user.PhoneNumberConfirmed,
                Roles = user.Roles,
            }).ToListAsync();
        }
        public async Task<string> GetFullName(ClaimsPrincipal User)
        {
            var UserInfo = await GetUserAsync(User);
            return UserInfo.FirstName + " " + UserInfo.LastName;
        }
        public async Task<List<UsersViewModel>> GetPaginateUsersAsync(int offset, int limit, string orderBy, string searchText)
        {
            var users = await Users.Include(u => u.Roles).Where(t => t.FirstName.Contains(searchText) || t.LastName.Contains(searchText) || t.Email.Contains(searchText) || t.UserName.Contains(searchText))
                    .Select(user => new UsersViewModel
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        CreditCardNumber = user.CreditCardNumber,
                        PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                        PhoneNumber = user.PhoneNumber,
                        RegisterDateTime = user.RegisterDateTime,
                        RoleId = user.Roles.Select(t=>t.Role.Id).FirstOrDefault(),
                        RoleName = user.Roles.Select(t=>t.Role.Name).FirstOrDefault(),
                    }).OrderBy(orderBy).Skip(offset).Take(limit).ToListAsync();
            foreach (var item in users)
            {
                item.PersianRegisterDateTime = item.RegisterDateTime.ConvertMiladiToShamsi("hh:mm:ss || yyyy/MM/dd");
                item.Row = ++offset;
            }
            return users;
        }
        public async Task<UsersViewModel> FindUserWithRolesByIdAsync(int UserId)
        {
            return await Users.Where(u => u.Id == UserId).Select(user => new UsersViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RegisterDateTime = user.RegisterDateTime,
                Roles = user.Roles,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                CreditCardNumber=user.CreditCardNumber,
            }).FirstOrDefaultAsync();
        }
        public async Task<bool> UserAccessToAdminPanel(string userName)
        {
            var Role = await Users.Where(t=>t.UserName==userName).Include(t=>t.Roles).ThenInclude(t=>t.Role).Select(t=>t.Roles.FirstOrDefault().Role).FirstOrDefaultAsync();
            if (Role.Id!=3)
                return true;
            return false;
        }
    }
}
