using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.System
{
    public class Sys_Cog_Permission
    {
        public int Id { get; set; }
        public int GroupRoleId { get; set; }
        public string MenuId { get; set; }
        public string RouterLink { get; set; }
        public bool ViewPer { get; set; }
        public bool EditPer { get; set; }
        public bool AddPer { get; set; }
        public bool DelPer { get; set; }
        public bool ExportPer { get; set; }
        public int CompanyId { get; set; }
        public int DepartmentId { get; set; }
        public bool IsActive { get; set; }
        public int UserCreateId { get; set; }
        public DateTime CreatDate { get; set; }
        public string ParentId { get; set; }
    }
}
