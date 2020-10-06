using HumanResource.Data.Entities.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.DTO
{
   public class UserGroupRole
    {
        public Sys_Dm_User sys_Dm_User { get; set; }
        public List<Sys_Dm_GroupRole> GroupRoles { get; set; }
    }
}
