using HumanResource.Application.Helper.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HumanResoureAPI.Common
{
    public class CommonData
    {
        public static RequestToken GetDataFromToken(ClaimsPrincipal claims)
        {
            string tokendata = claims.Claims.First(c => c.Type == "User").Value;
            var jsondata = JsonConvert.DeserializeObject<RequestToken>(tokendata);
            return jsondata;
        }
    }
}
