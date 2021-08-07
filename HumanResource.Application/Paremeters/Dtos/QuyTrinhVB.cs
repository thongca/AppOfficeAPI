using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Application.Paremeters.Dtos
{
   public class QuyTrinhVB
    {
        public int CompanyId { get; set; }
        public string QuyTrinhId { get; set; }
        public string BuocId { get; set; }
        public string LenhTuongTacId { get; set; }
    }

    public class BuocLenhGroupRole
    {
        public int CompanyId { get; set; }
        public string BuocLenhTuongTacId { get; set; }
        public string QuyTrinhId { get; set; }
        public string BuocId { get; set; }
        public string LenhTuongTacId { get; set; }
        public int GroupRoleId { get; set; }
        public bool? IsAll { get; set; }
        public bool? IsDepartment { get; set; }
        public bool? IsNest { get; set; }
        public bool? IsAllComCon { get; set; }
        public bool? IsAllComCha { get; set; }
        public bool? IsNguoiLap { get; set; }
        public bool? IsNguoiGui { get; set; }
        public bool? IsManagement { get; set; }
    }
}
