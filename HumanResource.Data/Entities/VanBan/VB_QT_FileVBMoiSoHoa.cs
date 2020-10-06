using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.VanBan
{
   public class VB_QT_FileVBMoiSoHoa
    { 
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string VbMoiSoHoaId { get; set; }
        public double Size { get; set; }
        public int Type { get; set; }
    }
}
