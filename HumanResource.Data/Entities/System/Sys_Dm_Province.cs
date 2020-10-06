using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.System
{
    public class Sys_Dm_Province
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
        public int IsOrder { get; set; }
    }
}
