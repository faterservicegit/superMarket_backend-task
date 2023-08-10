using AutoMapper;
using FaterRasanehMarket.Common;
using FaterRasanehMarket.Data.Contracts;
using FaterRasanehMarket.Entities.identity;
using FaterRasanehMarket.Services.Contracts;
using FaterRasanehMarket.ViewModels.Identity;
using FaterRasanehMarket.Common.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FaterRasanehMarket.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "مدیریت")]
    public class UserController : Controller
    {
        private readonly IApplicationUserManager _userManager;
        private readonly IApplicationRoleManager _roleManager;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _UW;

        private const string UserNotFound = "کاربر یافت نشد.";
        public UserController(IApplicationUserManager userManager, IApplicationRoleManager roleManager, IMapper mapper, IUnitOfWork UW)
        {
            _userManager = userManager;
            _userManager.CheckArgumentIsNull(nameof(_userManager));

            _roleManager = roleManager;
            _roleManager.CheckArgumentIsNull(nameof(_roleManager));

            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
            _UW = UW;
            _UW.CheckArgumentIsNull(nameof(_UW));
        }
        [HttpGet, DisplayName("مشاهده کاربران")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet, AjaxOnly]
        public async Task<IActionResult> GetUsers(string search, string order, int offset, int limit, string sort)
        {
            List<UsersViewModel> Users;
            int total = _userManager.Users.Count();
            if (string.IsNullOrWhiteSpace(search))
                search = "";
            if (limit == 0)
                limit = total;
            if (sort == "نام")
            {
                if (order == "asc")
                    Users = await _userManager.GetPaginateUsersAsync(offset, limit, "FirstName", search);
                else
                    Users = await _userManager.GetPaginateUsersAsync(offset, limit, "FirstName desc", search);
            }
            else if (sort == "نام خانوادگی")
            {
                if (order == "asc")
                    Users = await _userManager.GetPaginateUsersAsync(offset, limit, "LastName", search);
                else
                    Users = await _userManager.GetPaginateUsersAsync(offset, limit, "LastName desc", search);
            }
            else if (sort == "وضعیت")
            {
                if (order == "asc")
                    Users = await _userManager.GetPaginateUsersAsync(offset, limit, "Email", search);
                else
                    Users = await _userManager.GetPaginateUsersAsync(offset, limit, "Email desc", search);
            }
            else if (sort == "نام کاربری")
            {
                if (order == "asc")
                    Users = await _userManager.GetPaginateUsersAsync(offset, limit, "UserName", search);
                else
                    Users = await _userManager.GetPaginateUsersAsync(offset, limit, "UserName desc", search);
            }
            else if (sort == "تاریخ عضویت" && order == "asc")
            {
                Users = await _userManager.GetPaginateUsersAsync(offset, limit, "RegisterDateTime", search);
            }
            else
                Users = await _userManager.GetPaginateUsersAsync(offset, limit, "RegisterDateTime desc", search);
            if (search != "")
                total = Users.Count();
            return Json(new { total, rows = Users });
        }
        [HttpGet, AjaxOnly]
        [DisplayName("درج/ویرایش")]
        public async Task<IActionResult> RenderUser(int? userId)
        {
            var userViewModel = new UsersViewModel();
            ViewBag.Roles = _roleManager.GetAllRoles();
            if (userId != null)
            {
                var user = await _userManager.FindUserWithRolesByIdAsync((int)userId);
                if (User != null)
                {
                    userViewModel = _mapper.Map<UsersViewModel>(user);
                }
                else
                    ModelState.AddModelError(string.Empty, UserNotFound);
            }
            return PartialView("_RenderUser", userViewModel);
        }
        [HttpPost, AjaxOnly]
        [DisplayName("درج و ویرایش")]
        public async Task<IActionResult> CreateOrUpdate(UsersViewModel viewModel)
        {
            ViewBag.Roles = _roleManager.GetAllRoles();
            if (viewModel.Id != null)
            {
                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");
            }
            if (ModelState.IsValid)
            {
                IdentityResult Result = new IdentityResult();
                viewModel.Roles = new List<UserRole> { new UserRole { RoleId = (int)viewModel.RoleId } };
                if (viewModel.Id != null)
                {
                    var user = await _userManager.FindByIdAsync(viewModel.Id.ToString());
                    if (user != null)
                    {
                        var userRoles = await _userManager.GetRolesAsync(user);
                        if (userRoles.Count > 0)
                            await _userManager.RemoveFromRolesAsync(user, userRoles);
                        else
                            user.SecurityStamp = Guid.NewGuid().ToString();
                        user.FirstName = viewModel.FirstName;
                        user.LastName = viewModel.LastName;
                        user.Email = viewModel.Email;
                        user.UserName = viewModel.PhoneNumber;
                        user.PhoneNumber = viewModel.PhoneNumber;
                        user.Roles = viewModel.Roles;
                        user.PhoneNumberConfirmed = viewModel.PhoneNumberConfirmed;
                        user.CreditCardNumber = viewModel.CreditCardNumber;
                        Result = await _userManager.UpdateAsync(user);
                        if (Result.Succeeded)
                            TempData["notification"] = "ویرایش کاربر با موفقیت انجام شد.";
                    }
                }
                else
                {
                    var user = _mapper.Map<User>(viewModel);
                    user.UserName = viewModel.PhoneNumber;
                    user.Roles = viewModel.Roles;
                    Result = await _userManager.CreateAsync(user, viewModel.Password);
                    if (Result.Succeeded)
                    {
                        await _UW.Commit();
                        TempData["notification"] = "کاربر با موفقیت اضافه شد.";
                    }
                }
                if (!Result.Succeeded)
                {
                    ModelState.AddErrorsFromResult(Result);
                }
            }
            return PartialView("_RenderUser", viewModel);
        }
        [HttpGet, AjaxOnly]
        [DisplayName("حذف")]
        public async Task<IActionResult> Delete(int? userId)
        {
            if (userId == null)
                ModelState.AddModelError("", UserNotFound);
            else
            {
                var User = await _userManager.FindByIdAsync(userId.ToString());
                if (User == null)
                    ModelState.AddModelError("", UserNotFound);
                else
                    return View("_DeleteConfirmation", User);
            }
            return View("_DeleteConfirmation");
        }
        [HttpPost, ActionName("Delete"), AjaxOnly]
        public async Task<IActionResult> DeleteConfirmed(User model)
        {
            var User = await _userManager.FindByIdAsync(model.Id.ToString());
            if (User != null)
            {

                var result = await _userManager.DeleteAsync(User);
                if (result.Succeeded)
                {
                    TempData["notification"] = "حذف کاربر با موفقیت انجام شد.";
                    return PartialView("_DeleteConfirmation", model);
                }
                else
                    ModelState.AddErrorsFromResult(result);
            }
            else
                ModelState.AddModelError("", UserNotFound);
            return PartialView("_DeleteConfirmation");
        }
        [HttpPost, AjaxOnly, ActionName("DeleteGroup")]
        [DisplayName("حذف گروهی")]
        public async Task<IActionResult> DeletedGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError("", "هیچ کاربری برای حذف انتخاب نشده است.");
            else
            {
                foreach (var item in btSelectItem)
                {
                    var user = await _userManager.FindByIdAsync(item);
                    await _userManager.DeleteAsync(user);
                }
                TempData["notification"] = "حذف گروهی با موفقیت انجام شد.";
            }
            return PartialView("_DeleteGroup");
        }
        [HttpGet]
        [DisplayName("تغییر رمز کاربران"), AjaxOnly]
        public async Task<IActionResult> ChangeUserPassword(int? userId)
        {
            var passwordViewModel = new ChangePasswordViewModel();
            if (userId != null)
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (User != null)
                {
                    passwordViewModel.Id = user.Id;
                }
                else
                    ModelState.AddModelError(string.Empty, UserNotFound);
            }
            return PartialView("_ChangePassword", passwordViewModel);
        }
        [HttpPost, AjaxOnly]
        public async Task<IActionResult> ChangeUserPasswordConfirm(ChangePasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(viewModel.Id.ToString());
                if (User != null)
                {
                    var result = await _userManager.RemovePasswordAsync(user);
                    if (result.Succeeded)
                        result = await _userManager.AddPasswordAsync(user, viewModel.Password);
                    if (result.Succeeded)
                        TempData["notification"] = "تغییر کلمه عبور با موفقیت انجام شد.";
                }
                else
                    ModelState.AddModelError(string.Empty, UserNotFound);
            }
            return PartialView("_ChangePassword", viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> UserReports(int? userId)
        {
            ViewBag.SelectedUser = null;
            ViewBag.Users = await _UW.BaseRepository<User>().FindAllAsync();
            if (userId != null)
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user != null)
                    ViewBag.SelectedUser = user.Id;
            }
            return View();
        }
        [HttpGet]
        [DisplayName("گزارش کاربران")]
        public async Task<IActionResult> UserReportDetails(int userId)
        {
            var viewModel = new UserReportsViewModel
            {
                UserDetails = await _UW._Context.Users.Where(t => t.Id == userId).Include(t => t.Roles).ThenInclude(t => t.Role).Select(t => new UserDetails
                {
                    Id = t.Id,
                    FirstName = t.FirstName,
                    LastName = t.LastName,
                    PhoneNumber=t.PhoneNumber,
                    BuyAmount= t.Orders.Where(t=>t.CustomerId==userId).Sum(t=>t.AmountPaid),
                    VisitCount=t.Visits.Where(t=>t.UserId==userId).Sum(t=>t.NumberOfVisit),
                    RoleName = t.Roles.FirstOrDefault().Role.Name,
                    RegisterDateTime = t.RegisterDateTime.ConvertMiladiToShamsi("yyyy/MM/dd"),
                }).FirstOrDefaultAsync(),
            };
            return PartialView("_UserReport", viewModel);
        }
    }
}
