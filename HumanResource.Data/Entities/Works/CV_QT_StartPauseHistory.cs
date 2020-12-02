using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.Works
{
   public class CV_QT_StartPauseHistory
    {
        public int Id { get; set; }
        public string MyWorkId { get; set; }
        public DateTime CreateDate { get; set; }
        public int CycleWork { get; set; }
        public int UserCreateId { get; set; }
        public bool? Done { get; set; }
    }
}
