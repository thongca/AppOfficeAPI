using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.Works
{
   public class CV_DM_Error
    {
        public int Id { get; set; }
        public string ErrorName { get; set; }
        public double Point { get; set; }
        public int DepartmentId { get; set; }
    }
}
