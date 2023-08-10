using FaterRasanehWebSite.Common;
using FaterRasanehWebSite.Data.Contracts;
using FaterRasanehWebSite.Entities;
using FaterRasanehWebSite.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FaterRasanehWebSite.Areas.Admin.ViewComponents
{
    [ViewComponent(Name = "DataStatistics")]

    public class DataStatistics : ViewComponent
    {
        private readonly IUnitOfWork _UW;
        public DataStatistics(IUnitOfWork UW)
        {
            _UW = UW;
            _UW.CheckArgumentIsNull(nameof(_UW));
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var year = DateTimeExtensions.ConvertMiladiToShamsi(DateTime.Now, "yyyy");
            var currentMonth = DateTimeExtensions.ConvertMiladiToShamsi(DateTime.Now, "MMMM");
            var Month = StringExtensions.GetMonth();
            var numOfCurrentMonth = Month.ToList().IndexOf(currentMonth)+1;
            var Data = new DataStatisticsViewModel() { Year = year, Month = Month.ToList() };
            DateTime StartDateTimeMiladi = DateTimeExtensions.ConvertShamsiToMiladi($"{year}/1/01");
            DateTime EndDateTimeMiladi;
            int musicsTotalCount = await _UW._Context.Musics.CountAsync(t => (t.File != null || t.Url != null) & t.CreateDateTime < StartDateTimeMiladi);
            int videosTotalCount = await _UW._Context.Videos.CountAsync(t => (t.File != null || t.Url != null) & t.CreateDateTime < StartDateTimeMiladi);
            for (int i = 0; i < numOfCurrentMonth; i++)
            {
                StartDateTimeMiladi = DateTimeExtensions.ConvertShamsiToMiladi($"{year}/{i + 1}/01");
                if (i < 11)
                    EndDateTimeMiladi = DateTimeExtensions.ConvertShamsiToMiladi($"{year}/{i + 2}/01");
                else
                    EndDateTimeMiladi = DateTimeExtensions.ConvertShamsiToMiladi($"{year}/01/01");
                var currentcount =
                musicsTotalCount += await _UW._Context.Musics.CountAsync(t => (t.File != null || t.Url != null) & t.CreateDateTime >= StartDateTimeMiladi & t.CreateDateTime <= EndDateTimeMiladi);
                Data.MusicesMonthCount.Add(musicsTotalCount);
                videosTotalCount += await _UW._Context.Videos.CountAsync(t => (t.File != null || t.Url != null) & t.CreateDateTime >= StartDateTimeMiladi & t.CreateDateTime <= EndDateTimeMiladi);
                Data.VideosMonthCount.Add(videosTotalCount);
            }
            Data.DataCount.Add(await _UW._Context.Videos.CountAsync(t => t.File != null || t.Url != null));
            Data.DataCount.Add(await _UW._Context.Musics.CountAsync(t => t.File != null || t.Url != null));
            Data.DataCount.Add(_UW.BaseRepository<IPTV>().CountEntities());
            Data.DataCount.Add(_UW.BaseRepository<CCTV>().CountEntities());
            return View(Data);
        }
    }
}
