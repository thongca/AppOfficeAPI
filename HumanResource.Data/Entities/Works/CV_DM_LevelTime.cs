using HumanResource.Data.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.Works
{
   public class CV_DM_LevelTime
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Point { get; set; }
        public int CompanyId { get; set; }
        public int DepartmentId { get; set; }
        public Nullable<bool> Deleted { get; set; }
        public CV_DM_LevelTime()
        {

        }
        public CV_DM_LevelTime(CV_DM_LevelTimeRequest request)
        {
            Name = request.Name;
            Point = request.Point;
        }
    }
}
