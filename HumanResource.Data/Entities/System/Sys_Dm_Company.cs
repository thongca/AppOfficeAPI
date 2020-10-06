using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.System
{
   public class Sys_Dm_Company
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Website { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Logo { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public int Creator { get; set; }
        public int IsOrder { get; set; }
        public int? ParentId { get; set; }
        public int LevelCom { get; set; }
    }
}
