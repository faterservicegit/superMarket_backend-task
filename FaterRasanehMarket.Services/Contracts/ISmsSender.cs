using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FaterRasanehMarket.Services
{
    public interface ISmsSender
    {
        Task<bool> SendAuthSmsAsync(string Code, string PhoneNumber);
    }
}
