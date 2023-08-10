using Coravel.Invocable;
using FaterRasanehMarket.Data.Contracts;
using FaterRasanehMarket.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace FaterRasanehMarket.Services
{
    public class RegisterDayVisits : IInvocable
    {
        private readonly IUnitOfWork _UW;

        public RegisterDayVisits(IUnitOfWork UW)
        {
            _UW = UW;
        }

        public async Task Invoke()
        {
            var visitsCount = await _UW._Context.Visits.CountAsync(t => t.LastVisitDateTime >= DateTime.Now.Date);
            await _UW.BaseRepository<DayVisit>().CreateAsync(new DayVisit { DateTime = DateTime.Now.Date, VisitCount = visitsCount });
            await _UW.Commit();
        }
    }
}
