using Microsoft.AspNetCore.Mvc;
using FaterRasanehWebSite.Data.Contracts;
using System.Threading.Tasks;
using FaterRasanehWebSite.Entities;
using System;
using System.Linq;
using FaterRasanehWebSite.Common;

namespace FaterRasanehWebSite.Areas.Admin.ViewComponents
{
    [ViewComponent(Name = "LastComments")]
    public class LastComments:ViewComponent
    {
        private readonly IUnitOfWork _UW;
        public LastComments(IUnitOfWork UW)
        {
            _UW = UW;
            _UW.CheckArgumentIsNull(nameof(_UW));
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View((await _UW.BaseRepository<Comment>().FindByConditionAsync(t=>t.IsConfirm==false&&t.PostageDateTime.Value.AddDays(1)>=DateTime.Now,t=>t.OrderByDescending(v=>v.PostageDateTime),t=>t.User)).ToList());
        }
    }
}
