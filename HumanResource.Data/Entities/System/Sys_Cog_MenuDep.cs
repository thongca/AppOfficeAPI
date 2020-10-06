using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.System
{
   public class Sys_Cog_MenuDep
    {
        public int Id { get; set; }
        public string MenuId { get; set; }
        public int DepartmentId { get; set; }
        public int CompanyId { get; set; }
        public bool IsActive { get; set; }
        public string ParentId { get; set; }
        public int UserUpdateId { get; set; }
        public DateTime DateUpdate { get; set; }
    }
}
