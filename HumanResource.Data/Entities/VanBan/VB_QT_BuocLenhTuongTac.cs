using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.VanBan
{
   public class VB_QT_BuocLenhTuongTac
    {
        public string Id { get; set; }
        public string BuocId { get; set; }
        public string LenhTuongTacId { get; set; }
        public int IsOrder { get; set; }
        public int CompanyId { get; set; }
    }
}
