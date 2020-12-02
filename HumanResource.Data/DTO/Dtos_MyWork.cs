using HumanResource.Data.Entities.Works;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.DTO
{
   public class Dtos_MyWork
    {
        public CV_QT_MyWork CV_QT_MyWork { get; set; }
        public List<CV_QT_MySupportWork> CV_QT_MySupportWorks { get; set; }
        public List<CV_QT_DepartmentSupporter> CV_QT_DepartmentSupporter { get; set; }
        public int SpaceTimeId { get; set; }
    }
    public class Dtos_FlowWork
    {
        public CV_QT_WorkFlow CV_QT_WorkFlow { get; set; }
        public CV_QT_MySupportWork CV_QT_NextPlan { get; set; }
        public List<CV_QT_MySupportWork> CV_QT_CCUsers { get; set; }
    }
    public class Dtos_FlowWorkPheDuyetTH
    {
        public CV_QT_WorkFlow CV_QT_WorkFlow { get; set; }
        public DateTime? ChangeDate { get; set; }
    }
}
