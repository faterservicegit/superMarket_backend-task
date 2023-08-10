using FaterRasanehWebSite.Common;
using FaterRasanehWebSite.Data.Contracts;
using FaterRasanehWebSite.ViewModels;
using MD.PersianDateTime.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace FaterRasanehWebSite.Areas.Admin.ViewComponents
{
    public class VisitsStatistics : ViewComponent
    {
        private readonly IUnitOfWork _UW;
        public VisitsStatistics(IUnitOfWork UW)
        {
            _UW = UW;
            _UW.CheckArgumentIsNull(nameof(_UW));
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var persianDateTime = new PersianDateTime(DateTime.Now);
            var Model = new VisitsStatisticsViewModel()
            {
                Month = persianDateTime.MonthName,
            };
            var today = persianDateTime.Day;
            for (int day = 1; day <= today; day++)
            {
                Model.Days.Add(day);
                DateTime StartDateTimeMiladi = DateTimeExtensions.ConvertShamsiToMiladi($"{persianDateTime.Year}/{persianDateTime.Month}/{day}");
                Model.UsersCount.Add(await _UW._Context.Users.CountAsync(t => t.RegisterDateTime <= StartDateTimeMiladi));
                Model.VideosVisitsCount.Add(await _UW._Context.VideoVisits.CountAsync(t => t.LastVisitDateTime <= StartDateTimeMiladi.AddDays(1) & t.LastVisitDateTime > StartDateTimeMiladi));
                Model.MusicesVisitsCount.Add(await _UW._Context.MusicVisits.CountAsync(t => t.LastVisitDateTime <= StartDateTimeMiladi.AddDays(1) & t.LastVisitDateTime > StartDateTimeMiladi));
            }
            return View(Model);
        }
    }
}
