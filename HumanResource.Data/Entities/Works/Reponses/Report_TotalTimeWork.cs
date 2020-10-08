using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HumanResource.Data.Entities.Works.Reponses
{
   public class Report_TotalTimeWork
    {
        [Key]
        public string TaskCode { get; set; }
        public string TaskName { get; set; }
        public double? TongLk { get; set; }
        public double? ChuTriLk { get; set; }
        public double? PhoiHopLk { get; set; }
        public double? NgayLk { get; set; }
        public double? Tong { get; set; }
        public double? ChuTri { get; set; }
        public double? PhoiHop { get; set; }
        public double? WorkNgay { get; set; }
    }
}
