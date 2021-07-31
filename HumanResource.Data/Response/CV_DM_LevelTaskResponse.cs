﻿using HumanResource.Data.Entities.Works;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Response
{
   public class CV_DM_LevelTaskResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Point { get; set; }
        public int CompanyId { get; set; }
        public int DepartmentId { get; set; }
        public Nullable<bool> Deleted { get; set; }
        public CV_DM_LevelTaskResponse()
        {

        }
        public CV_DM_LevelTaskResponse(CV_DM_LevelTask response)
        {
            Id = response.Id;
            Name = response.Name;
            Point = response.Point;
        }
    }
}
