using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Application.Paremeters.Dtos
{
   public class QuyTrinhVB
    {
        public int CompanyId { get; set; }
        public int QuyTrinhId { get; set; }
        public int? BuocId { get; set; }
        public int? LenhTuongTacId { get; set; }
    }

    public class BuocLenhGroupRole
    {
        public int CompanyId { get; set; }
        public int BuocLenhTuongTacId { get; set; }
        public int QuyTrinhId { get; set; }
        public int BuocId { get; set; }
        public int LenhTuongTacId { get; set; }
        public int GroupRoleId { get; set; }
        public bool? IsAll { get; set; }
        public bool? IsDepartment { get; set; }
        public bool? IsNest { get; set; }
        public bool? IsAllComCon { get; set; }
        public bool? IsAllComCha { get; set; }
        public bool? IsNguoiLap { get; set; }
        public bool? IsNguoiGui { get; set; }
    }
}
