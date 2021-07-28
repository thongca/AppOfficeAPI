using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.System
{
    public class Sys_Dm_GroupRole
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? CompanyId { get; set; }
        public bool IsActive { get; set; }
        public int IsOrder { get; set; }
        public int? DepartmentId { get; set; }
        public DateTime CreateDate { get; set; }
        public int UserCreateId { get; set; }
    }
}
