using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantAPI2.IntegrationTests.Helpers
{
    public static class HttpContentHelper
    {
        public static HttpContent ToJsonHttpContent(this object obj) //dowolny obiekt w pamięci będzie mógłbyć serializowany typu httpContent za pomocą tej metody
        {
            var json = JsonConvert.SerializeObject(obj);

            var httpContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            return httpContent;
        }
    }
}
