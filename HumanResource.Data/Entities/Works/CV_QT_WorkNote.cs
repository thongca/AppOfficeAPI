using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.Works
{
   public class CV_QT_WorkNote
    {
        public int Id { get; set; }
        public string MyWorkId { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public double WorkTime { get; set; }
        public int CreatedBy { get; set; }
        public bool? OverTime { get; set; }
        public bool? Handle { get; set; }
        public DateTime? HandleDate { get; set; }
        public int? HandleUserId { get; set; }
        public int? State { get; set; }
        public int CompanyId { get; set; }
    }
}
