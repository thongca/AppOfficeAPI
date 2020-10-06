using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.VanBan
{
   public class VB_QT_BuocLenhGroupRole
    {
        public int Id { get; set; }
        public int GroupRoleId { get; set; }
        public int BuocLenhTuongTacId { get; set; }
        public bool IsAll { get; set; }
        public bool IsDepartment { get; set; }
        public bool IsNest { get; set; }
        public bool IsAllComCon { get; set; }
        public bool IsAllComCha { get; set; }
        public bool IsNguoiLap { get; set; }
        public bool IsNguoiGui { get; set; }
    }
}
