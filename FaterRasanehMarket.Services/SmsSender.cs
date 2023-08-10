using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FaterRasanehMarket.Services
{
    public class SmsSender : ISmsSender
    {
        public async Task<bool> SendAuthSmsAsync(string Code,string PhoneNumber)
        {
            HttpClient httpClient = new HttpClient();
            var httpResponse = await httpClient.GetAsync($"https://api.kavenegar.com/v1/6B765436533127685367598878782487165446D35335A72505A3177567943/verify/lookup.json?receptor={PhoneNumber}&token={Code}&template=AuthVerify");
            if (httpResponse.StatusCode == HttpStatusCode.OK)
                return true;
            else
                return false;
        }
    }
}
