using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.Works
{
   public class CV_QT_MyScheduleWork
    {
        public int Id { get; set; }
        public string TaskName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int UserDeliverId { get; set; }
        public string FullName { get; set; }
        public int UserCreateId { get; set; }
        public int Predecessor { get; set; }
        public double? WorkTime { get; set; }
        public string MyWorkId { get; set; }
        public int StatusWork { get; set; }
        public Nullable<DateTime>  DateComplete { get; set; }
        public int? UserUpdateId { get; set; }
        public Nullable<DateTime> UpdateDate { get; set; }
    }
}
