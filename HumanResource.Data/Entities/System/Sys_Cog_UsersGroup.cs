using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.System
{
   public class Sys_Cog_UsersGroup
    {
        public int Id { get; set; }
        public int GroupRoleId { get; set; }
        public int UserId { get; set; }
    }
}
