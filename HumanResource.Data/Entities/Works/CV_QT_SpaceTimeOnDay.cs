using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.Works
{
   public class CV_QT_SpaceTimeOnDay
    {
        public int Id { get; set; }
        public double SpaceStart { get; set; }
        public double SpaceEnd { get; set; }
        public double Time { get; set; }
        public string MyWorkId { get; set; }
        public int UserId { get; set; }
        public bool? Handled { get; set; }
        public DateTime? HandledDate { get; set; }
        public int? HandledUserId { get; set; }
        public string MyWorkNewId { get; set; }
    }
}
