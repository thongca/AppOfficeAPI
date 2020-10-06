using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HumanResource.Data.Entities.Works.Reponses
{
   public class RePort_KpiForUseraMonth
    {
        [Key]
        public string TaskCode { get; set; }
        public string TaskName { get; set; }
        public double PointTask { get; set; }
        public double PointTime { get; set; }
        public double WorkTime { get; set; }
        public double Point { get; set; }
        public string ChildrenError { get; set; }
    }
}
