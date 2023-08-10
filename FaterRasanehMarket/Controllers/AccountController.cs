using FaterRasanehMarket.Common;
using FaterRasanehMarket.Common.Attributes;
using FaterRasanehMarket.Data.Contracts;
using FaterRasanehMarket.Entities;
using FaterRasanehMarket.Entities.identity;
using FaterRasanehMarket.Services;
using FaterRasanehMarket.Services.Contracts;
using FaterRasanehMarket.ViewModels.Identity;
using FaterRasanehMarket.ViewModels.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FaterRasanehMarket.Controllers
{
    public class AccountController : Controller
    {
        private readonly IApplicationUserManager _userManager;
        private readonly IApplicationRoleManager _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUnitOfWork _UW;
        private readonly ISmsSender _smsSender;
        private const string UserNotFound = "نام کاربری اشتباه است.";
        public AccountController(IApplicationUserManager userManager, IApplicationRoleManager roleManager, SignInManager<User> signInManager, ISmsSender smsSender, IUnitOfWork UW)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _smsSender = smsSender;
            _UW = UW;
        }
        [Route("[action]")]
        [HttpGet]
        public IActionResult Login(string ReturnUrl)
        {
            return View(new SignInViewModel { ReturnUrl = ReturnUrl });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("[action]")]
        public async Task<IActionResult> Login(SignInViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var User = await _userManager.FindByNameAsync(viewModel.UserName);
                if (User == null)
                    ModelState.AddModelError("", UserNotFound);
                else
                {
                    //var Token = await _userManager.GenerateChangePhoneNumberTokenAsync(User, "09104599475");
                    //var Reesult = await _userManager.ChangePhoneNumberAsync(User, "09104599475", Token);
                    var Result = await _userManager.CheckPasswordAsync(User, viewModel.Password);
                    if (Result == true)
                    {
                        var result = await _signInManager.PasswordSignInAsync(User, viewModel.Password, viewModel.RememberMe, true);
                        if (result.Succeeded)
                        {
                            if (viewModel.ReturnUrl != null)
                            {
                                if (!await _userManager.UserAccessToAdminPanel(User.UserName))
                                {
                                    if (viewModel.ReturnUrl.ToLowerInvariant().Contains("admin"))
                                        return RedirectToAction("Index", "Home");
                                }
                                return Redirect(viewModel.ReturnUrl);
                            }
                            return RedirectToAction("Index", "Home");
                        }
                        else if (result.IsLockedOut)
                            ModelState.AddModelError(string.Empty, "حساب کاربری شما به مدت 20 دقیقه به دلیل تلاش های ناموفق قفل شد.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "کلمه عبور شما صحیح نمی باشد.");
                    }
                }
            }
            return View(viewModel);
        }
        [Route("[action]")]
        public IActionResult Register(bool ForgotPassword=false)
        {
            var viewModel = new RegisterViewModel { ForgotPassword = ForgotPassword };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("[action]")]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(viewModel.PhoneNumber);
                if (user == null)
                {
                    user = new User { UserName = viewModel.PhoneNumber };
                    await _userManager.CreateAsync(user);
                }
                else
                {
                    if (user.PhoneNumberConfirmed == true)
                        ModelState.AddModelError("", "با این شماره قبلا ثبت نام کرده اید");
                    return View(viewModel);
                }
                var Token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, viewModel.PhoneNumber);
                await _smsSender.SendAuthSmsAsync(Token, viewModel.PhoneNumber);
                //var Result = await _userManager.ChangePhoneNumberAsync(user, "09104599475", Token);
            }
            else
                ModelState.AddModelError("", "شماره وارد شده نامعتبر است");
            return View(viewModel);
        }
        [HttpGet, AjaxOnly]
        [Route("[action]")]
        [Authorize]
        public IActionResult Logout()
        {
            return View();
        }
        [HttpPost, ActionName("Logout")]
        [Route("[action]")]
        [Authorize]
        public async Task<IActionResult> LogoutConfirmed()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/Home");
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            var viewModel = new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                CreditCardNumber = user.CreditCardNumber,
                Addresses = await _UW.CartAndOrdersRepository.GetUserAddressesAsync(user.Id)
            };
            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Profile(ProfileViewModel viewModel)
        {
            if (viewModel.OldPassword == null || viewModel.Password == null)
            {
                ModelState.Remove("OldPassword");
                ModelState.Remove("Password");
                ModelState.Remove("ConfimePassword");
            }
            else
                viewModel.PasswordProblem = true;
            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            if (ModelState.IsValid)
            {
                user.FirstName = viewModel.FirstName;
                user.LastName = viewModel.LastName;
                user.Email = viewModel.Email;
                await _UW.Commit();
                if (viewModel.OldPassword != null & viewModel.Password != null)
                {
                    var result = await _userManager.ChangePasswordAsync(user, viewModel.OldPassword, viewModel.Password);
                    if (!result.Succeeded)
                        ModelState.AddErrorsFromResult(result);
                }
                TempData["notification"] = "پروفایل با موفقیت بروزرسانی شد";
            }
            viewModel.Addresses = await _UW.CartAndOrdersRepository.GetUserAddressesAsync(user.Id);
            return View("Profile", viewModel);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RenderAddress(AddressViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.Id != null)
                {
                    var address = await _UW.BaseRepository<Address>().FindByIdAsync(viewModel.Id);
                    if (address == null)
                        return NotFound();
                    address.FullAddress = viewModel.FullAddress;
                    address.Title = viewModel.Title;
                    address.PhoneNumber = viewModel.PhoneNumber;
                    await _UW.Commit();
                    TempData["notification"] = "ویرایش آدرس با موفقیت انجام شد.";
                }
                else
                {
                    var addresses= await _UW.CartAndOrdersRepository.GetUserAddressesAsync(int.Parse(User.Identity.GetUserId()));
                    await _UW.BaseRepository<Address>().CreateAsync(new Address
                    {
                        UserId = int.Parse(User.Identity.GetUserId()),
                        Title = viewModel.Title,
                        FullAddress = viewModel.FullAddress,
                        IsDefualt=addresses.Count==0,
                        PhoneNumber = viewModel.PhoneNumber,
                    });
                    await _UW.Commit();
                    TempData["notification"] = "درج آدرس با موفقیت انجام شد.";
                }
            }
            else
            {
                TempData["badnotification"] = "آدرس  نامعتبر است.";
            }
            return RedirectToAction("Profile");
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DeleteAddress(int addressId)
        {
            var selectedAddress = (await _UW.BaseRepository<Address>().FindByConditionAsync(t=>t.Id==addressId&t.UserId==int.Parse(User.Identity.GetUserId()),null,t=>t.Orders)).FirstOrDefault();
            if (selectedAddress == null)
                return BadRequest("آدرس مورد نظر یافت نشد!");
            _UW.BaseRepository<Address>().Delete(selectedAddress);
            await _UW.Commit();
            return Ok("آدرس با موفقیت حذف شد");
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SetDefualtAddress(int addressId)
        {
            var Addresses = await _UW.CartAndOrdersRepository.GetUserAddressesAsync(int.Parse(User.Identity.GetUserId()));
            var selectedAddress = Addresses.Where(t => t.Id == addressId).FirstOrDefault();
            if (selectedAddress == null)
                return BadRequest("آدرس مورد نظر یافت نشد!");
            foreach(var address in Addresses)
            {
                address.IsDefualt = false;
            }
            selectedAddress.IsDefualt = true;
            await _UW.Commit();
            return Ok("آدرس با موفقیت بعنوان پیشفرض انتخاب شد");
        }
    }
}
