using HumanResource.Data.Entities.VanBan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HumanResource.Application.Helper;
namespace HumanResoureAPI.Common
{
    public static class LuanChuyenVanBan
    {
        public static VB_QT_LuanChuyenVanBan r2AddLuanChuyenVanBan(string VbMoiSoHoaId, int? NguoiNhanId, string TenNguoiNhan, int NguoiGuiId, string TenNguoiGui, string TieuDe, string NoiDung, bool? UuTien, DateTime? HanXuLy, DateTime? NgayXuLy,int TrangThaiXuLy, string MaLenh, DateTime? NgayDoc, bool TinhTrang, string ParentId, string MenuGuiId, string MenuNhanId, string PositionNN, string PositionNG, string DepartmentNN, string DepartmentNG)
        {
            VB_QT_LuanChuyenVanBan obj = new VB_QT_LuanChuyenVanBan();
            obj.Id = Helper.GenKey();
            obj.VbMoiSoHoaId = VbMoiSoHoaId;
            obj.NguoiGuiId = NguoiGuiId;
            obj.TenNguoiGui = TenNguoiGui;
            obj.NguoiNhanId = NguoiNhanId;
            obj.TenNguoiNhan = TenNguoiNhan;
            obj.ThoiGianGui = DateTime.Now;
            obj.TieuDe = TieuDe;
            obj.TieuDeKhongDau = TieuDe;
            obj.NoiDung = NoiDung;
            obj.XuLyUuTien = UuTien;
            obj.HanXuLy = HanXuLy;
            obj.NgayXuLy = null;
            obj.DaDoc = false;
            obj.NgayDoc = NgayDoc;
            obj.TinhTrang = TinhTrang;
            obj.ParentId = ParentId;
            obj.MenuGuiId = MenuGuiId;
            obj.MenuNhanId = MenuNhanId;
            obj.MaLenh = MaLenh;
            obj.TrangThaiXuLy = TrangThaiXuLy;
            obj.PositionNN = PositionNN;
            obj.PositionNG = PositionNG;
            obj.DepartmentNG = DepartmentNG;
            obj.DepartmentNN = DepartmentNN;
            return obj;
        }
    }
}
