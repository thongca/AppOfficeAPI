using HumanResource.Data.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.Works
{
   public class CV_DM_LevelTask
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Point { get; set; }
        public int CompanyId { get; set; }
        public int DepartmentId { get; set; }
        public Nullable<bool> Deleted { get; set; }
        public CV_DM_LevelTask()
        {

        }
        public CV_DM_LevelTask(CV_DM_LevelTaskRequest request)
        {
            Name = request.Name;
            Point = request.Point;
        }
    }
}
