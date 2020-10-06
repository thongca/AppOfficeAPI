using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.System
{
   public class Sys_Dm_Department
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public int  CompanyId { get; set; }
        public int IsOrder { get; set; }
        public bool IsActive { get; set; }

    }
}
