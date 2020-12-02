using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.Works
{
   public class CV_QT_DepartmentSupporter
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public string MyWorkId { get; set; }
        public double? CreatedDate { get; set; }
        public string DepartmentName { get; set; }
    }
}
