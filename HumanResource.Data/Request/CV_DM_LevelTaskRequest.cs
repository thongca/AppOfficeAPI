using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Request
{
   public class CV_DM_LevelTaskRequest: SearchBase
    {
        public Nullable<int> Id { get; set; }
        public string Name { get; set; }
        public double Point { get; set; }
        public int CompanyId { get; set; }
        public int DepartmentId { get; set; }
        public Nullable<bool> Deleted { get; set; }
    }
}
