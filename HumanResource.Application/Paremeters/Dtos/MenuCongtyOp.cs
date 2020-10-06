using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Application.Paremeters.Dtos
{
   public class MenuCongtyOp
    {
        public int CompanyId { get; set; }
        public string Id { get; set; }
        public bool IsActive { get; set; }
        public int DepartmentId { get; set; }
    }
    public class MenuCongtyNest
    {
        public int CompanyId { get; set; }
        public string Id { get; set; }
        public bool IsActive { get; set; }
        public int DepartmentId { get; set; }
        public int NestId { get; set; }
    }
    public class MenuPermissionCom
    {
        public int CompanyId { get; set; }
        public string MenuId { get; set; }
        public bool? AddPer { get; set; }
        public bool? ViewPer { get; set; }
        public bool? EditPer { get; set; }
        public bool? DelPer { get; set; }
        public bool? ExportPer { get; set; }
        public int DepartmentId { get; set; }
        public int GroupRoleId { get; set; }
        public int NestId { get; set; }
    }
    public class Menu
    {
        public string Id { get; set; }
  
        public bool Active { get; set; }

    }
}
