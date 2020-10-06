using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.VanBan
{
   public class VB_QT_BuocLenhTuongTac
    {
        public int Id { get; set; }
        public int BuocId { get; set; }
        public int LenhTuongTacId { get; set; }
        public int IsOrder { get; set; }
    }
}
