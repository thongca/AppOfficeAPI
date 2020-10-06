using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.VanBan
{
   public class VB_QT_LuanChuyenVanBan
    {
        public string Id { get; set; }
        public string VbMoiSoHoaId { get; set; }
        public int NguoiGuiId { get; set; }
        public string TenNguoiGui { get; set; }
        public int? NguoiNhanId { get; set; }
        public string TenNguoiNhan { get; set; }
        public DateTime ThoiGianGui { get; set; }
        public string TieuDe { get; set; }
        public string TieuDeKhongDau { get; set; }
        public string NoiDung { get; set; }
        public bool? XuLyUuTien { get; set; }
        public DateTime? HanXuLy { get; set; }
        public DateTime? NgayXuLy { get; set; }
        public int TrangThaiXuLy { get; set; }
        public string MaLenh { get; set; }
        public bool DaDoc { get; set; }
        public bool TinhTrang { get; set; }
        public DateTime? NgayDoc { get; set; }
        public string ParentId { get; set; }
        public string MenuGuiId { get; set; }
        public string MenuNhanId { get; set; }
        public string PositionNG { get; set; }
        public string DepartmentNG { get; set; }
        public string PositionNN { get; set; }
        public string DepartmentNN { get; set; }
    }
}
