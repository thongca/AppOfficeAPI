using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.Works
{
   public class CV_DM_DefaultTask
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? GroupTaskId { get; set; }
        public int Frequency { get; set; }
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// Mức độ công việc
        /// </summary>
        public Byte LevelTask { get; set; }
        /// <summary>
        /// Mức độ ưu tiên
        /// </summary>
        public Byte LevelTime { get; set; }
        public double PointTask { get; set; }
        public double PointTime { get; set; }
        public int? DepartmentId { get; set; }
    }
}
