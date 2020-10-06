using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.Works
{
   public class CV_QT_CounterError
    {
        public int Id { get; set; }
        public string MyWorkId { get; set; }
        public string FlowWorkId { get; set; }
        public int ErrorId { get; set; }
        public double Point { get; set; }
        public int NguoiBiPhatId { get; set; }
        public int NguoiPhatId { get; set; }
        public DateTime CreateDate { get; set; }
        public int DepartmentId { get; set; }
        public int TypeUserDeli { get; set; }
    }
}
