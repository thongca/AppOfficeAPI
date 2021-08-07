using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.Works
{
    public class CV_QT_MyWork
    {
        public string Id { get; set; }
        public int Code { get; set; }
        public string TaskId { get; set; }
        public string TaskCode { get; set; }
        public string TaskName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int UserTaskId { get; set; }
        public string UserTaskName { get; set; }
        public int TypeTask { get; set; }
        public int? DeliverType { get; set; }
        public DateTime? StartPause { get; set; }
        public DateTime? EndPause { get; set; }
        public double? PauseTime { get; set; }
        public double? WorkTime { get; set; }
        public int CycleWork { get; set; }
        public int DepartmentId { get; set; }
        public string Note { get; set; }
        public int? Predecessor { get; set; }
        public DateTime? PreWorkDeadline { get; set; }
        public Nullable<bool> PreWorkType { get; set; }
        public Nullable<DateTime> CompleteDate { get; set; }
        public Nullable<int> TypeComplete { get; set; }
        public int LevelTaskId { get; set; }
        public int LevelTimeId { get; set; }
        public Nullable<double> PointTask { get; set; }
        public Nullable<double> PointTime { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public int? Repossibility { get; set; }
        public Nullable<int> ReporterId { get; set; }
        public string ReporterName { get; set; }
        public int CompanyId { get; set; }
    }
}
