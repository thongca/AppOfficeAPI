using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.System
{
   public class Sys_Dm_Position
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
    }
}
