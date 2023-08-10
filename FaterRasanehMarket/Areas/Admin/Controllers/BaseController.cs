using FaterRasanehMarket.Common.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace FaterRasanehMarket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BaseController : Controller
    {


        [HttpGet, AjaxOnly]
        public IActionResult Notification()
        {
            if (TempData["notification"] != null)
                return Content(TempData["notification"].ToString());
            else
                return Content("عملیات با موفقیت انجام شد.");
        }
        [HttpGet, AjaxOnly]
        public IActionResult DeleteGroup()
        {
            return PartialView("_DeleteGroup");
        }

        [HttpGet]
        [Route("[Area]/AccessDenied")]
        public IActionResult AccessDenied()
        {
            if (User.IsInRole("کاربر"))
                return NotFound();
            return View();
        }
        [HttpGet]
        [Route("/Error404")]
        public IActionResult Error404()
        {
            return View();
        }
        [HttpGet]
        [Route("/Error")]
        public IActionResult Error()
        {
            return View();
        }
    }
}
