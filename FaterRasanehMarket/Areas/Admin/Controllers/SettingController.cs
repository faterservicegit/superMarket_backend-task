using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using FaterRasanehMarket.Common;
using FaterRasanehMarket.Services.Contracts;
using FaterRasanehMarket.ViewModels.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace FaterRasanehMarket.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "مدیریت")]
    public class SettingController : Controller
    {
        private readonly IWritableOptions<SiteSettings> _writableLocations;
        private readonly IWebHostEnvironment _env;
        public SettingController( IWebHostEnvironment env, IWritableOptions<SiteSettings> writableLocations)
        {
            _env = env;
            _writableLocations = writableLocations;
        }
        [HttpGet, DisplayName("تنظیمات سایت")]
        public IActionResult SiteSetting()
        {
            var settings = new SettingsViewModel()
            {
                Title = _writableLocations.Value.SiteInfo.Title,
                Description = _writableLocations.Value.SiteInfo.Description,
                Email = _writableLocations.Value.SiteInfo.Email,
                PhoneNumber = _writableLocations.Value.SiteInfo.PhoneNumber,
                Address= _writableLocations.Value.SiteInfo.Address,
            };
            return View(settings);
        }
        [HttpGet, DisplayName("سایر تنظیمات")]
        public IActionResult OtherSettings()
        {
            var settings = new OtherSettingsViewModel()
            {
                EmailSettings =new EmailSettingsViewModel {
                    Email= _writableLocations.Value.EmailSettings.Email,
                    Username = _writableLocations.Value.EmailSettings.Username,
                    Password = _writableLocations.Value.EmailSettings.Password,
                    Host = _writableLocations.Value.EmailSettings.Host,
                    Port = _writableLocations.Value.EmailSettings.Port,
                },
                OrderSettings = new OrderSettingsViewModel
                {
                    OfflinePayment = _writableLocations.Value.OrderSettings.OfflinePayment,
                    OnlinePayment = _writableLocations.Value.OrderSettings.OnlinePayment,
                    IsTakingOrder = _writableLocations.Value.OrderSettings.IsTakingOrder,
                    ShippingCost = _writableLocations.Value.OrderSettings.ShippingCost,
                    MinOrderForFreeShipping = _writableLocations.Value.OrderSettings.MinOrderForFreeShipping,
                },
            };
            return View(settings);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SiteSetting(SettingsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.Favicon != null)
                   await viewModel.Favicon.UploadFileAsync($"{_env.WebRootPath}/Main/image/favicon.ico");
                if (viewModel.Logo != null)
                    await viewModel.Logo.UploadFileAsync($"{_env.WebRootPath}/Main/image/logo.png");
                _writableLocations.Update(opt =>
                {
                    opt.SiteInfo.Title = viewModel.Title;
                    opt.SiteInfo.Description = viewModel.Description;
                    opt.SiteInfo.Address = viewModel.Address;
                    opt.SiteInfo.Email = viewModel.Email;
                    opt.SiteInfo.PhoneNumber = viewModel.PhoneNumber;
                });
                ViewBag.Alert = "ویرایش تنظیمات با موفقیت انجام شد.";
            }
            return View(viewModel);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult OtherSettings(OtherSettingsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                _writableLocations.Update(opt =>
                {
                    opt.OrderSettings.IsTakingOrder = viewModel.OrderSettings.IsTakingOrder;
                    opt.OrderSettings.OfflinePayment = viewModel.OrderSettings.OfflinePayment;
                    opt.OrderSettings.OnlinePayment = viewModel.OrderSettings.OnlinePayment;
                    opt.OrderSettings.ShippingCost = viewModel.OrderSettings.ShippingCost;
                    opt.OrderSettings.MinOrderForFreeShipping = viewModel.OrderSettings.MinOrderForFreeShipping;
                    opt.EmailSettings.Email = viewModel.EmailSettings.Email;
                    opt.EmailSettings.Username = viewModel.EmailSettings.Username;
                    opt.EmailSettings.Password = viewModel.EmailSettings.Password;
                    opt.EmailSettings.Host = viewModel.EmailSettings.Host;
                    opt.EmailSettings.Port = viewModel.EmailSettings.Port;
                });
                ViewBag.Alert = "ویرایش تنظیمات با موفقیت انجام شد.";
            }
            return View(viewModel);
        }
        [HttpGet, DisplayName("سایر تنظیمات")]
        public IActionResult CloseOrOpenShop()
        {
            var message = "فروشگاه درحال سفارش گیری";
            if(_writableLocations.Value.OrderSettings.IsTakingOrder==true)
                message = "فروشگاه دیگر سفارشی نمی گیرد";
            _writableLocations.Update(opt =>
            {
                opt.OrderSettings.IsTakingOrder = !opt.OrderSettings.IsTakingOrder;
            });
            return Ok(message);
        }
    }
}
