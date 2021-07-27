using HumanResource.Data.DTO;
using HumanResource.Data.EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumanResoureAPI.Common
{
    public static class CheckPermission
    {

        public static List<Sys_Dm_GroupRoleLogin> getListGroupRole(humanDbContext context, int UserId)
        {
            List<Sys_Dm_GroupRoleLogin> userRole = (List<Sys_Dm_GroupRoleLogin>)(from a in context.Sys_Cog_UsersGroup
                           join b in context.Sys_Dm_GroupRole on a.GroupRoleId equals b.Id
                           where a.UserId == UserId
                           orderby b.RankRole
                           select new
                           {
                               a.GroupRoleId,
                           });
            return userRole;
        }
    }
}
