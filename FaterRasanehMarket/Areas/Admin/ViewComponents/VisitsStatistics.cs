using FaterRasanehMarket.Common;
using FaterRasanehMarket.Data.Contracts;
using FaterRasanehMarket.ViewModels;
using MD.PersianDateTime.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FaterRasanehMarket.Areas.Admin.ViewComponents
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
            var persianDateTime = new PersianDateTime(DateTime.UtcNow.Date);
            var Model = new VisitsStatisticsViewModel()
            {
                Month = persianDateTime.MonthName,
            };
            var today = persianDateTime.Day;
            for (int day = 1; day <= today; day++)
            {
                Model.Days.Add(day);
                DateTime StartDateTimeMiladi = DateTimeExtensions.ConvertShamsiToMiladi($"{persianDateTime.Year}/{persianDateTime.Month}/{day}");
                Model.ProductsCount.Add(await _UW._Context.Products.CountAsync(t => t.AddDateTime.Date < StartDateTimeMiladi.AddDays(1).Date&t.IsDeleted==false));
                if (day < today)
                    Model.VisistsCount.Add(await _UW._Context.DayVisits.Where(t => t.DateTime.Date == StartDateTimeMiladi.Date).Select(t => t.VisitCount).FirstOrDefaultAsync());
                else
                    Model.VisistsCount.Add(await _UW._Context.Visits.Where(t => t.LastVisitDateTime >= StartDateTimeMiladi.Date).CountAsync());
            }
            return View(Model);
        }
    }
}
