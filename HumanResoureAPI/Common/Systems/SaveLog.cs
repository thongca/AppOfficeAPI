using HumanResource.Data.Request;using HumanResource.Data.EF;
using HumanResource.Data.Entities.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumanResoureAPI.Common.Systems
{
    public class SaveLog
    {
        public static bool SaveLogEx(humanDbContext dbContext, string urlApi, string errorLog, string Description)
        {
            Sys_TK_SaveLog sys_TK_SaveLog = new Sys_TK_SaveLog()
            {
                UrlApi = urlApi,
                ErrorLog = errorLog,
                Description = Description
            };
            dbContext.Sys_TK_SaveLog.Add(sys_TK_SaveLog);
            dbContext.SaveChanges();
            return true;
        }
    }
}
