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
       public static int CheckPer(humanDbContext context, int UserId, int GroupRoleId)
        {
            var user = context.Sys_Cog_UsersGroup.FirstOrDefault(x=>x.UserId == UserId);
            if (user == null)
            {
                return 6; // chưa tồn tài nhóm quyền
            }
            var groupRole = context.Sys_Dm_GroupRole.Find(GroupRoleId);
            if (groupRole.IsAdministrator == true)
            {
                return 0; // admin tổng
            }
            else if (groupRole.IsAdminCom == true)
            {
                return 1;// admin công ty
            }
            else if (groupRole.IsAdminDep == true)
            {
                return 2;// admin phòng
            }
            else if (groupRole.IsAdminNest == true)
            {
                return 3;// admin Tổ
            }
            else if (groupRole.CompanyId != 0 && groupRole.IsAdminCom == null && groupRole.IsAdminDep == null && groupRole.IsAdminNest == null)
            {
                return 4;// role thường
            }
            else if (groupRole.CompanyId != 0 && groupRole.IsAdminCom == false && groupRole.IsAdminDep == false && groupRole.IsAdminNest == false)
            {
                return 4;// role thường
            }
            return 6;
        }
        public static int getGroupRoleDefault(humanDbContext context, int UserId)
        {
            var userRole = from a in context.Sys_Cog_UsersGroup
                       join b in context.Sys_Dm_GroupRole on a.GroupRoleId equals b.Id
                       where a.UserId == UserId
                       orderby b.RankRole
                       select new
                       {
                           a.GroupRoleId,
                       };
            if (userRole.Count() == 0)
            {
                return 0; // chưa có role
            } else
            {
                return userRole.FirstOrDefault().GroupRoleId;
            }       
        }
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
