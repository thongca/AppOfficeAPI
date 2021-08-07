using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Request
{
   public class Sys_Dm_DepartmentRequest: SearchBase
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public int CompanyId { get; set; }
        public int IsOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
