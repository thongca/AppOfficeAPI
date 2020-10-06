using HumanResource.Data.Entities.VanBan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumanResoureAPI.HelperPara
{
    public class LuanChuyenVbUser
    {
        public VB_QT_LuanChuyenVanBan VB_QT_LuanChuyenVanBan { get; set; }
        public UserNhan UserNhan { get; set; }
    }
    public class UserNhan
    {
        public int? NguoiChiDaoId { get; set; }
        public string TenNguoiChiDao { get; set; }
        public int? NguoiXuLyId { get; set; }
        public string TenNguoiXuLy { get; set; }
        public int? NguoiDXuLyId { get; set; }
        public string TenNguoiDXuLy { get; set; }
        public int? NguoiNDBId { get; set; }
        public string TenNguoiNDB { get; set; }
    }
    public class LuanChuyenVbDuyetTrinhKy
    {
        public VB_QT_LuanChuyenVanBan VB_QT_LuanChuyenVanBan { get; set; }
        public UserXNHT UserXNHT { get; set; }
    }
    public class UserXNHT
    {
        public bool? isNguoiLap { get; set; }
        public bool? isNguoiGui { get; set; }
    }
}
