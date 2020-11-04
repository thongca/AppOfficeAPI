using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.Online
{
   public class Sys_Dm_Lisesion
    {
        public int Id { get; set; }
        public bool Login { get; set; }
        public DateTime HanDung { get; set; }
    }
}
