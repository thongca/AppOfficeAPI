using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.System
{
   public class Sys_QT_ThongBao
    {
        public int Id { get; set; }
        public int QuyTrinhId { get; set; }
        public string MaLenh { get; set; }
        public string TenNguoiGui { get; set; }
        public string NoiDung { get; set; }
        public DateTime NgayGui { get; set; }
        public int NguoiNhanId { get; set; }
        public bool DaDoc { get; set; }
        public DateTime? NgayDoc { get; set; }
        public int TrangThaiXuLy { get; set; }
        public string RouterLink { get; set; }
        public string TrangThai { get; set; }
        public bool IsNotifi { get; set; }
    }
}
