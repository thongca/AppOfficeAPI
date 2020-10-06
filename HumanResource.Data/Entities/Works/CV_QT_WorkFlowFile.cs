using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.Works
{
   public class CV_QT_WorkFlowFile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public double Size { get; set; }
        public string Extension { get; set; }
        public string WorkFlowId { get; set; }
    }
}
