using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HumanResource.Application.Helper.Dtos;
using HumanResource.Application.Paremeters.Dtos;
using HumanResource.Data.Request;using HumanResource.Data.EF;
using HumanResource.Data.Entities.System;
using HumanResource.Data.Entities.VanBan;
using HumanResoureAPI.Common;
using HumanResoureAPI.HelperPara;
using HumanResoureAPI.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HumanResoureAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VanBanChoPheDuyetController : ControllerBase
    {
        private readonly humanDbContext _context;
        public VanBanChoPheDuyetController(humanDbContext context)
        {
            _context = context;
        }
        #region Danh sách văn bản chờ phê duyệt
        // Get: api/VanBanChoPheDuyet/r1GetListVanBanChoPheDuyet
        [HttpGet]
        [Route("r1GetListVanBanChoPheDuyet")]
        public async Task<ActionResult<IEnumerable<VB_QT_LuanChuyenVanBan>>> r1GetListVanBanChoPheDuyet()
        {
             RequestToken token = CommonData.GetDataFromToken(User);
            var tables =await (from a in _context.VB_QT_LuanChuyenVanBan
                          join b in _context.VB_QT_VanBanMoiSoHoa on a.VbMoiSoHoaId equals b.Id
                          where a.NguoiNhanId == token.UserID && a.NgayXuLy == null && a.MaLenh == "VB_CHOTRINHKY"
                          orderby a.ThoiGianGui descending
                          select new
                          {
                              a.ThoiGianGui,
                              a.TenNguoiGui,
                              a.MaLenh,
                              b.TrichYeu,
                              b.SoKyHieu,
                              b.NgayBanHanh,
                              b.CreateDate,
                              a.Id,
                              a.VbMoiSoHoaId
                          }).ToListAsync();

            return new ObjectResult(new { error = 0, data = tables });

        }
        #endregion
        #region Lưu quy trình phê duyệt văn bản
        // Post: api/VanBanChoPheDuyet/r2AddQTPheDuyetTrinhKy
        [HttpPost]
        [Route("r2AddQTPheDuyetTrinhKy")]
        public async Task<ActionResult<IEnumerable<VB_QT_VanBanMoiSoHoa>>> r2AddQTPheDuyetTrinhKy(LuanChuyenVbDuyetTrinhKy luanChuyenVbXNHT)
        {
            try
            {
                List<NguoiNhanThongBao> nhanThongBaos = new List<NguoiNhanThongBao>();
                 RequestToken token = CommonData.GetDataFromToken(User);
                var user = await _context.Sys_Dm_User.FindAsync(token.UserID);
                var qtLuanChuyenVb =await _context.VB_QT_LuanChuyenVanBan.FindAsync(luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.Id);
               
                qtLuanChuyenVb.NgayXuLy = DateTime.Now;
                qtLuanChuyenVb.NgayDoc = DateTime.Now;
                qtLuanChuyenVb.DaDoc = true;

                var nguoiNhans = _context.VB_QT_LuanChuyenVanBan.Where(x => x.VbMoiSoHoaId == qtLuanChuyenVb.VbMoiSoHoaId).Select(a => new
                {
                    a.TenNguoiGui,
                    a.NguoiGuiId,
                    a.MaLenh
                }).Distinct().ToList();
                if (luanChuyenVbXNHT.UserXNHT.isNguoiGui == true)
                {
                    foreach (var item in nguoiNhans.Where(x => x.MaLenh == "VB_CHOTRINHKY" && x.NguoiGuiId != nguoiNhans.FirstOrDefault(x => x.MaLenh == "VB_MOISOHOA").NguoiGuiId))
                    {
                        var userNN = await _context.Sys_Dm_User.FindAsync(item.NguoiGuiId);
                        VB_QT_LuanChuyenVanBan lcvb = LuanChuyenVanBan.r2AddLuanChuyenVanBan(
                          qtLuanChuyenVb.VbMoiSoHoaId, item.NguoiGuiId, item.TenNguoiGui, token.UserID, user.FullName,
                          luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.TieuDe,
                          luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.NoiDung, false,
                          luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.HanXuLy, null,
                          5,
                          "VB_NHANTHONGBAO", null, false,
                          qtLuanChuyenVb.Id,
                          luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.MenuGuiId,
                          luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.MenuNhanId,
                          userNN.PositionName, user.PositionName, userNN.DepartmentName, user.DepartmentName);
                            _context.VB_QT_LuanChuyenVanBan.Add(lcvb);
                        // thêm vào thông báo
                        Sys_QT_ThongBao obj = new Sys_QT_ThongBao();
                        obj.MaLenh = "VB_NHANTHONGBAO";
                        obj.QuyTrinhId = 1;
                        obj.TenNguoiGui = user.FullName;
                        obj.NguoiNhanId = item.NguoiGuiId;
                        obj.NoiDung = luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.NoiDung;
                        obj.NgayGui = DateTime.Now;
                        obj.DaDoc = false;
                        obj.NgayDoc = null;
                        obj.TrangThaiXuLy = 5;
                        obj.RouterLink = "/vanban/quytrinhvanban/vanbandapheduyet";
                        NguoiNhanThongBao nguoiNhan = new NguoiNhanThongBao();
                        nguoiNhan.NguoiNhanId = item.NguoiGuiId;
                        nhanThongBaos.Add(nguoiNhan);
                        _context.Sys_QT_ThongBao.Add(obj);
                    }
                }
                var userNNN = await _context.Sys_Dm_User.FindAsync(nguoiNhans.FirstOrDefault(x => x.MaLenh == "VB_MOISOHOA").NguoiGuiId);
                VB_QT_LuanChuyenVanBan lcvb1 = LuanChuyenVanBan.r2AddLuanChuyenVanBan(
                  qtLuanChuyenVb.VbMoiSoHoaId, nguoiNhans.FirstOrDefault(x=>x.MaLenh == "VB_MOISOHOA").NguoiGuiId, nguoiNhans.FirstOrDefault(x => x.MaLenh == "VB_MOISOHOA").TenNguoiGui, token.UserID, user.FullName,
                  luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.TieuDe,
                  luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.NoiDung, false,
                  luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.HanXuLy, null,
                  3,
                  luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.MaLenh, null, false,
                  qtLuanChuyenVb.Id,
                  luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.MenuGuiId,
                  luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.MenuNhanId,
                  userNNN.PositionName, user.PositionName, userNNN.DepartmentName, user.DepartmentName);
                    _context.VB_QT_LuanChuyenVanBan.Add(lcvb1);
                NguoiNhanThongBao nguoiNhan1 = new NguoiNhanThongBao();
                nguoiNhan1.NguoiNhanId = nguoiNhans.FirstOrDefault(x => x.MaLenh == "VB_MOISOHOA").NguoiGuiId;
                nhanThongBaos.Add(nguoiNhan1);
                // thêm vào thông báo
                Sys_QT_ThongBao obj1 = new Sys_QT_ThongBao();
                obj1.MaLenh = luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.MaLenh;
                obj1.QuyTrinhId = 1;
                obj1.TenNguoiGui = user.FullName;
                obj1.NguoiNhanId = nguoiNhans.FirstOrDefault(x => x.MaLenh == "VB_MOISOHOA").NguoiGuiId;
                obj1.NoiDung = luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.NoiDung;
                obj1.NgayGui = DateTime.Now;
                obj1.DaDoc = false;
                obj1.NgayDoc = null;
                obj1.TrangThaiXuLy = luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.TrangThaiXuLy;
                obj1.RouterLink = "/vanban/quytrinhvanban/vanbandapheduyet";
                _context.Sys_QT_ThongBao.Add(obj1);
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0, nguoiNhanTbs = nhanThongBaos });
            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }


        }
        #endregion

        #region Lưu quy trình trả lại trình ký
        // Post: api/VanBanChoPheDuyet/r2AddQTTraLaiTrinhKy
        [HttpPost]
        [Route("r2AddQTTraLaiTrinhKy")]
        public async Task<ActionResult<IEnumerable<VB_QT_VanBanMoiSoHoa>>> r2AddQTTraLaiTrinhKy(LuanChuyenVbDuyetTrinhKy luanChuyenVbXNHT)
        {
            try
            {
                List<NguoiNhanThongBao> nhanThongBaos = new List<NguoiNhanThongBao>();
                 RequestToken token = CommonData.GetDataFromToken(User);
                var user = await _context.Sys_Dm_User.FindAsync(token.UserID);
                var qtLuanChuyenVb = await _context.VB_QT_LuanChuyenVanBan.FindAsync(luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.Id);

                qtLuanChuyenVb.NgayXuLy = DateTime.Now;
                qtLuanChuyenVb.NgayDoc = DateTime.Now;
                qtLuanChuyenVb.DaDoc = true;

                var nguoiNhans = _context.VB_QT_LuanChuyenVanBan.Where(x => x.VbMoiSoHoaId == qtLuanChuyenVb.VbMoiSoHoaId).Select(a => new
                {
                    a.TenNguoiGui,
                    a.NguoiGuiId,
                    a.MaLenh
                }).Distinct();
                if (luanChuyenVbXNHT.UserXNHT.isNguoiGui == true)
                {
                    foreach (var item in nguoiNhans.Where(x => x.MaLenh == "VB_CHOTRINHKY" 
                    && x.NguoiGuiId != nguoiNhans.FirstOrDefault(x => x.MaLenh == "VB_MOISOHOA").NguoiGuiId
                    && x.NguoiGuiId != nguoiNhans.FirstOrDefault(x => x.MaLenh == "VB_BITRALAI").NguoiGuiId))
                    {
                        var userNN = await _context.Sys_Dm_User.FindAsync(item.NguoiGuiId);
                        VB_QT_LuanChuyenVanBan lcvb = LuanChuyenVanBan.r2AddLuanChuyenVanBan(
                        qtLuanChuyenVb.VbMoiSoHoaId, item.NguoiGuiId, item.TenNguoiGui, token.UserID, user.FullName,
                        luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.TieuDe,
                        luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.NoiDung, false,
                        luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.HanXuLy, null,
                        5,
                        "VB_NHANTHONGBAO", null, false,
                        qtLuanChuyenVb.Id,
                        luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.MenuGuiId,
                        luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.MenuNhanId,
                        userNN.PositionName, user.PositionName, userNN.DepartmentName, user.DepartmentName);
                        _context.VB_QT_LuanChuyenVanBan.Add(lcvb);
                        // thêm vào thông báo
                        Sys_QT_ThongBao obj = new Sys_QT_ThongBao();
                        obj.MaLenh = luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.MaLenh;
                        obj.QuyTrinhId = 1;
                        obj.TenNguoiGui = user.FullName;
                        obj.NguoiNhanId = item.NguoiGuiId;
                        obj.NoiDung = luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.NoiDung;
                        obj.NgayGui = DateTime.Now;
                        obj.DaDoc = false;
                        obj.NgayDoc = null;
                        obj.TrangThaiXuLy = 5;
                        obj.RouterLink = "/vanban/quytrinhvanban/vanbanbitralai";
                        NguoiNhanThongBao nguoiNhan = new NguoiNhanThongBao();
                        nguoiNhan.NguoiNhanId = item.NguoiGuiId;
                        nhanThongBaos.Add(nguoiNhan);
                        _context.Sys_QT_ThongBao.Add(obj);
                    }
                }
                var userNNN = await _context.Sys_Dm_User.FindAsync(nguoiNhans.FirstOrDefault(x => x.MaLenh == "VB_MOISOHOA").NguoiGuiId);
                VB_QT_LuanChuyenVanBan lcvb1 = LuanChuyenVanBan.r2AddLuanChuyenVanBan(
                qtLuanChuyenVb.VbMoiSoHoaId, nguoiNhans.FirstOrDefault(x => x.MaLenh == "VB_MOISOHOA").NguoiGuiId, nguoiNhans.FirstOrDefault(x => x.MaLenh == "VB_MOISOHOA").TenNguoiGui, token.UserID, user.FullName,
                luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.TieuDe,
                luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.NoiDung, false,
                luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.HanXuLy, null,
                4,
                luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.MaLenh, null, false,
                qtLuanChuyenVb.Id,
                luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.MenuGuiId,
                luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.MenuNhanId,
                userNNN.PositionName, user.PositionName, userNNN.DepartmentName, user.DepartmentName);
                _context.VB_QT_LuanChuyenVanBan.Add(lcvb1);
                NguoiNhanThongBao nguoiNhan1 = new NguoiNhanThongBao();
                nguoiNhan1.NguoiNhanId = nguoiNhans.FirstOrDefault(x => x.MaLenh == "VB_MOISOHOA").NguoiGuiId;
                nhanThongBaos.Add(nguoiNhan1);
                // thêm vào thông báo
                Sys_QT_ThongBao obj1 = new Sys_QT_ThongBao();
                obj1.MaLenh = luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.MaLenh;
                obj1.QuyTrinhId = 1;
                obj1.TenNguoiGui = user.FullName;
                obj1.NguoiNhanId = nguoiNhans.FirstOrDefault(x => x.MaLenh == "VB_MOISOHOA").NguoiGuiId;
                obj1.NoiDung = luanChuyenVbXNHT.VB_QT_LuanChuyenVanBan.NoiDung;
                obj1.NgayGui = DateTime.Now;
                obj1.DaDoc = false;
                obj1.NgayDoc = null;
                obj1.TrangThaiXuLy = 4;
                obj1.RouterLink = "/vanban/quytrinhvanban/vanbanbitralai";
                _context.Sys_QT_ThongBao.Add(obj1);
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0, nguoiNhanTbs = nhanThongBaos });
            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }


        }
        #endregion
    }
}