using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.System
{
   public class Sys_Dm_Menu
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string RouterLink { get; set; }
        public string ParentId { get; set; }
        public int IsOrder { get; set; }
        public bool IsActive { get; set; }
        public bool IsTitle { get; set; }
        public string IconMenu { get; set; }
        public int MenuRank { get; set; }
    }
}

