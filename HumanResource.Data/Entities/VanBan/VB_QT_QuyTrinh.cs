using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.VanBan
{
   public class VB_QT_QuyTrinh
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public bool IsActive { get; set; }
        public int IsOrder { get; set; }
        public int CompanyId { get; set; }
    }
}
