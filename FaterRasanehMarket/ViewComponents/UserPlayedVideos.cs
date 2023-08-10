using FaterRasanehWebSite.Common;
using FaterRasanehWebSite.Data.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FaterRasanehWebSite.Areas.Admin.ViewComponents
{
    public class UserPlayedVideos : ViewComponent
    {
        private readonly IUnitOfWork _UW;
        public UserPlayedVideos(IUnitOfWork UW)
        {
            _UW = UW;
            _UW.CheckArgumentIsNull(nameof(_UW));
        }
        public async Task<IViewComponentResult> InvokeAsync(int userId)
        {
            var Videos = await _UW._Context.VideoVisits.Where(t => t.UserId == userId).Include(t => t.Video).Select(t =>t.Video).ToListAsync();
            return View();
        }
    }
}
