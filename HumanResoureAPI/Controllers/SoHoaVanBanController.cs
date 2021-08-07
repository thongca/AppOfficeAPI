using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HumanResource.Application.Helper;
using HumanResource.Application.Helper.Dtos;
using HumanResource.Application.Paremeters.Dtos;
using HumanResource.Data.Request;
using HumanResource.Data.EF;
using HumanResource.Data.Entities.System;
using HumanResource.Data.Entities.VanBan;
using HumanResoureAPI.Common;
using HumanResoureAPI.HelperPara;
using HumanResoureAPI.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HumanResoureAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SoHoaVanBanController : ControllerBase
    {
        private readonly humanDbContext _context;
        public SoHoaVanBanController(humanDbContext context)
        {
            _context = context;
        }
        //Post: api/SoHoaVanBan/r2addObjvanBan
        [HttpPost, DisableRequestSizeLimit]
        [Route("r2addObjvanBan")]
        public async Task<IActionResult> r2addObjUnitCode()
        {
            try
            {
                var model = JsonConvert.DeserializeObject<VB_QT_VanBanMoiSoHoa>(Request.Form["model"]);
                VB_QT_VanBanMoiSoHoa objvb = new VB_QT_VanBanMoiSoHoa();
                RequestToken token = CommonData.GetDataFromToken(User);
                var user = await _context.Sys_Dm_User.FindAsync(token.UserID);
                var userNguoiKy = await _context.Sys_Dm_User.FirstOrDefaultAsync(x => x.Id == model.NguoiKyId);
                if (model != null)
                {
                    objvb.Id = Helper.GenKey();
                    objvb.CompanyId = user.CompanyId ?? 0;
                    objvb.DepartmentId = user.DepartmentId ?? 0;
                    objvb.TenNguoiKy = userNguoiKy.FullName;
                    objvb.LinhVucId = model.LinhVucId;
                    objvb.LoaiVanBanId = model.LoaiVanBanId;
                    objvb.SoKyHieu = model.SoKyHieu;
                    objvb.NoiBanHanh = model.NoiBanHanh;
                    objvb.NgayBanHanh = model.NgayBanHanh;
                    objvb.TuKhoa = model.TuKhoa;
                    objvb.SoTrang = model.SoTrang;
                    objvb.SoTrang = model.SoTrang;
                    objvb.TenNguoiTao = user.FullName;
                    objvb.CreateDate = DateTime.Now;
                    objvb.UserCreateId = token.UserID;
                    objvb.TrichYeu = model.TrichYeu;
                    _context.VB_QT_VanBanMoiSoHoa.Add(objvb);
                }
                VB_QT_LuanChuyenVanBan lcvb = LuanChuyenVanBan.r2AddLuanChuyenVanBan(objvb.Id, token.UserID, user.FullName, token.UserID, user.FullName, "", "", false, null, null, 1, "VB_MOISOHOA", DateTime.Now, false, null, "VB0101", "VB0101", user.PositionName, user.PositionName, user.DepartmentName, user.DepartmentName);
                _context.VB_QT_LuanChuyenVanBan.Add(lcvb);
                if (Request.Form.Files.Count != 0)
                {
                    foreach (var item in Request.Form.Files)
                    {
                        VB_QT_FileVBMoiSoHoa obj = new VB_QT_FileVBMoiSoHoa();
                        var file = item;
                        var folderName = Path.Combine("Resources", "FD" + token.CompanyId.ToString(), "VanBan");
                        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                        if (!Directory.Exists(pathToSave))
                        {
                            Directory.CreateDirectory(pathToSave);
                        }
                        if (model != null)
                        {
                            if (file.Length > 0)
                            {
                                var fileName = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")).ToString() + ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                                var fullPath = Path.Combine(pathToSave, fileName);
                                var dbPath = Path.Combine(folderName, fileName);
                                obj.Path = dbPath;
                                using (var stream = new FileStream(fullPath, FileMode.Create))
                                {
                                    file.CopyTo(stream);
                                }
                            }
                        }
                        obj.Name = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        obj.VbMoiSoHoaId = objvb.Id;
                        obj.Size = file.Length;
                        obj.Type = 1;
                        _context.VB_QT_FileVBMoiSoHoa.Add(obj);
                    }

                }
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0, ms = "" }); ;
            }
            catch (Exception ex)
            {
                var result = new OkObjectResult(new { error = 1, ms = "Lỗi khi thêm mới UnitCode, vui lòng kiểm tra lại!" });
                return result;
            }
        }
        #region lay 1 văn bản bằng Id
        // Get: api/SoHoaVanBan
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<VB_QT_VanBanMoiSoHoa>>> r1GetVanBanSoHoaById(string id)
        {
            var lcvb = await _context.VB_QT_LuanChuyenVanBan.FindAsync(id);
            if (lcvb.DaDoc != true)
            {
                lcvb.NgayDoc = DateTime.Now;
                lcvb.DaDoc = true;
                await _context.SaveChangesAsync();
            }
            RequestToken token = CommonData.GetDataFromToken(User);
            var tables = await _context.VB_QT_VanBanMoiSoHoa.FindAsync(id);
            var vb = await (from a in _context.VB_QT_VanBanMoiSoHoa
                            join b in _context.VB_Dm_LinhVuc on a.LinhVucId equals b.Id
                            join c in _context.VB_Dm_LoaiVanBan on a.LoaiVanBanId equals c.Id
                            where a.Id == lcvb.VbMoiSoHoaId
                            select new
                            {
                                a.Id,
                                a.NgayBanHanh,
                                a.SoKyHieu,
                                a.TrichYeu,
                                a.TenNguoiKy,
                                a.UserCreateId,
                                a.TenNguoiTao,
                                NameField = b.Name,
                                NameLoaiVb = c.Name,
                            }).ToListAsync();
            var files = _context.VB_QT_FileVBMoiSoHoa.Where(x => x.VbMoiSoHoaId == lcvb.VbMoiSoHoaId).Select(c => new
            {
                c.Name,
                c.Path,
                c.Size
            }).ToList();
            var lcvbs = _context.VB_QT_LuanChuyenVanBan.Where(x => x.VbMoiSoHoaId == lcvb.VbMoiSoHoaId).Select(c => new
            {
                c.Id,
                c.TenNguoiNhan,
                c.TenNguoiGui,
                c.ThoiGianGui,
                c.TrangThaiXuLy,
                c.NoiDung,
                c.NgayDoc,
                c.NgayXuLy,
                c.ParentId,
                c.DepartmentNN,
                c.PositionNN,
                c.PositionNG
            }).OrderByDescending(x => x.ThoiGianGui).ToList();

            return new ObjectResult(new { error = 0, data = vb[0], files, lcvbs, lcvb });
        }
        #endregion
        #region Danh sách văn bản mới số hóa
        // Get: api/SoHoaVanBan/r1GetListDanhSachVBSoHoa
        [HttpGet]
        [Route("r1GetListDanhSachVBSoHoa")]
        public async Task<ActionResult<IEnumerable<VB_QT_VanBanMoiSoHoa>>> r1GetListDanhSachVBSoHoa()
        {
            RequestToken token = CommonData.GetDataFromToken(User);
            var tables = from b in _context.VB_QT_LuanChuyenVanBan
                         join a in _context.VB_QT_VanBanMoiSoHoa on b.VbMoiSoHoaId equals a.Id
                         where b.MaLenh == "VB_MOISOHOA" && a.UserCreateId == token.UserID
                         select new
                         {
                             b.MaLenh,
                             a.TenNguoiTao,
                             b.Id,
                             b.VbMoiSoHoaId,
                             a.TrichYeu,
                             a.NgayBanHanh,
                             a.SoKyHieu,
                             a.CreateDate,
                             b.NgayXuLy
                         };
            return new ObjectResult(new { error = 0, data = await tables.OrderByDescending(x => x.CreateDate).ToListAsync() });

        }
        #endregion
        #region Lưu quy trình trình ký
        // Post: api/SoHoaVanBan/r2AddQTChuyenChoPheDuyet
        [HttpPost]
        [Route("r2AddQTChuyenChoPheDuyet")]
        public async Task<ActionResult<IEnumerable<VB_QT_VanBanMoiSoHoa>>> r2AddQTChuyenChoPheDuyet(VB_QT_LuanChuyenVanBan vB_QT_LuanChuyenVanBan)
        {
            try
            {
                RequestToken token = CommonData.GetDataFromToken(User);
                var user = await _context.Sys_Dm_User.FindAsync(token.UserID);
                var userNN = await _context.Sys_Dm_User.FindAsync(vB_QT_LuanChuyenVanBan.NguoiNhanId);
                var qtLuanChuyenVb = await _context.VB_QT_LuanChuyenVanBan.FindAsync(vB_QT_LuanChuyenVanBan.Id);
                qtLuanChuyenVb.NgayDoc = DateTime.Now;
                qtLuanChuyenVb.DaDoc = true;
                qtLuanChuyenVb.NgayXuLy = DateTime.Now;
                VB_QT_LuanChuyenVanBan lcvb = LuanChuyenVanBan.r2AddLuanChuyenVanBan(
                    qtLuanChuyenVb.VbMoiSoHoaId, vB_QT_LuanChuyenVanBan.NguoiNhanId, vB_QT_LuanChuyenVanBan.TenNguoiNhan, token.UserID, user.FullName,
                    vB_QT_LuanChuyenVanBan.TieuDe,
                    vB_QT_LuanChuyenVanBan.NoiDung, false,
                    vB_QT_LuanChuyenVanBan.HanXuLy, null,
                    vB_QT_LuanChuyenVanBan.TrangThaiXuLy,
                    vB_QT_LuanChuyenVanBan.MaLenh, null,
                    false, qtLuanChuyenVb.Id,
                    vB_QT_LuanChuyenVanBan.MenuGuiId,
                    vB_QT_LuanChuyenVanBan.MenuNhanId,
                    userNN.PositionName, user.PositionName, userNN.DepartmentName, user.DepartmentName);
                _context.VB_QT_LuanChuyenVanBan.Add(lcvb);
                Sys_QT_ThongBao obj = new Sys_QT_ThongBao();
                obj.MaLenh = vB_QT_LuanChuyenVanBan.MaLenh;
                obj.QuyTrinhId = 1;
                obj.TenNguoiGui = user.FullName;
                obj.NguoiNhanId = vB_QT_LuanChuyenVanBan.NguoiNhanId ?? 0;
                obj.NoiDung = vB_QT_LuanChuyenVanBan.NoiDung;
                obj.NgayGui = DateTime.Now;
                obj.DaDoc = false;
                obj.NgayDoc = null;
                obj.TrangThaiXuLy = vB_QT_LuanChuyenVanBan.TrangThaiXuLy;
                obj.RouterLink = "/vanban/quytrinhvanban/vanbanchopheduyet";
                _context.Sys_QT_ThongBao.Add(obj);
                List<NguoiNhanThongBao> nhanThongBaos = new List<NguoiNhanThongBao>();
                NguoiNhanThongBao nguoiNhan = new NguoiNhanThongBao();
                nguoiNhan.NguoiNhanId = vB_QT_LuanChuyenVanBan.NguoiNhanId ?? 0;
                nhanThongBaos.Add(nguoiNhan);
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0, nguoiNhanTbs = nhanThongBaos });
            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }


        }
        #endregion
        #region Lưu quy trình chuyển xử lý
        // Post: api/SoHoaVanBan/r2AddQTChuyenXuLy
        [HttpPost]
        [Route("r2AddQTChuyenXuLy")]
        public async Task<ActionResult<IEnumerable<VB_QT_VanBanMoiSoHoa>>> r2AddQTChuyenXuLy(LuanChuyenVbUser luanChuyenVbUser)
        {
            try
            {
                RequestToken token = CommonData.GetDataFromToken(User);
                var user = await _context.Sys_Dm_User.FindAsync(token.UserID);
                var userNCD = await _context.Sys_Dm_User.FindAsync(luanChuyenVbUser.UserNhan.NguoiChiDaoId);
                var userNXL = await _context.Sys_Dm_User.FindAsync(luanChuyenVbUser.UserNhan.NguoiXuLyId);
                var userNDXL = await _context.Sys_Dm_User.FindAsync(luanChuyenVbUser.UserNhan.NguoiDXuLyId);
                var userNNDB = await _context.Sys_Dm_User.FindAsync(luanChuyenVbUser.UserNhan.NguoiNDBId);

                var qtLuanChuyenVb = _context.VB_QT_LuanChuyenVanBan.Where(x => x.VbMoiSoHoaId == luanChuyenVbUser.VB_QT_LuanChuyenVanBan.VbMoiSoHoaId
                && x.NguoiNhanId == luanChuyenVbUser.VB_QT_LuanChuyenVanBan.NguoiGuiId
                && x.MenuNhanId == luanChuyenVbUser.VB_QT_LuanChuyenVanBan.MenuGuiId).OrderByDescending(x => x.ThoiGianGui).Take(1);
                if (luanChuyenVbUser.UserNhan.NguoiChiDaoId != null)
                {
                    VB_QT_LuanChuyenVanBan lcvb = LuanChuyenVanBan.r2AddLuanChuyenVanBan(
                  luanChuyenVbUser.VB_QT_LuanChuyenVanBan.VbMoiSoHoaId, luanChuyenVbUser.UserNhan.NguoiChiDaoId, luanChuyenVbUser.UserNhan.TenNguoiChiDao, token.UserID, user.FullName,
                  luanChuyenVbUser.VB_QT_LuanChuyenVanBan.TieuDe,
                  luanChuyenVbUser.VB_QT_LuanChuyenVanBan.NoiDung, false,
                  luanChuyenVbUser.VB_QT_LuanChuyenVanBan.HanXuLy, null,
                  5,
                  luanChuyenVbUser.VB_QT_LuanChuyenVanBan.MaLenh, null, false,
                  qtLuanChuyenVb.FirstOrDefault().Id,
                  luanChuyenVbUser.VB_QT_LuanChuyenVanBan.MenuGuiId,
                  luanChuyenVbUser.VB_QT_LuanChuyenVanBan.MenuNhanId,
                  userNCD.PositionName, user.PositionName, userNCD.DepartmentName, user.DepartmentName);
                    _context.VB_QT_LuanChuyenVanBan.Add(lcvb);
                }
                if (luanChuyenVbUser.UserNhan.NguoiXuLyId != null)
                {
                    VB_QT_LuanChuyenVanBan lcvb = LuanChuyenVanBan.r2AddLuanChuyenVanBan(
                  luanChuyenVbUser.VB_QT_LuanChuyenVanBan.VbMoiSoHoaId, luanChuyenVbUser.UserNhan.NguoiXuLyId, luanChuyenVbUser.UserNhan.TenNguoiXuLy, token.UserID, user.FullName,
                  luanChuyenVbUser.VB_QT_LuanChuyenVanBan.TieuDe,
                  luanChuyenVbUser.VB_QT_LuanChuyenVanBan.NoiDung, false,
                  luanChuyenVbUser.VB_QT_LuanChuyenVanBan.HanXuLy, null,
                  6,
                  luanChuyenVbUser.VB_QT_LuanChuyenVanBan.MaLenh, null, false,
                  qtLuanChuyenVb.FirstOrDefault().Id,
                  luanChuyenVbUser.VB_QT_LuanChuyenVanBan.MenuGuiId,
                  luanChuyenVbUser.VB_QT_LuanChuyenVanBan.MenuNhanId,
                   userNXL.PositionName, user.PositionName, userNXL.DepartmentName, user.DepartmentName);
                    _context.VB_QT_LuanChuyenVanBan.Add(lcvb);
                }
                if (luanChuyenVbUser.UserNhan.NguoiDXuLyId != null)
                {
                    VB_QT_LuanChuyenVanBan lcvb = LuanChuyenVanBan.r2AddLuanChuyenVanBan(
                  luanChuyenVbUser.VB_QT_LuanChuyenVanBan.VbMoiSoHoaId, luanChuyenVbUser.UserNhan.NguoiDXuLyId, luanChuyenVbUser.UserNhan.TenNguoiDXuLy, token.UserID, user.FullName,
                  luanChuyenVbUser.VB_QT_LuanChuyenVanBan.TieuDe,
                  luanChuyenVbUser.VB_QT_LuanChuyenVanBan.NoiDung, false,
                  luanChuyenVbUser.VB_QT_LuanChuyenVanBan.HanXuLy, null,
                  7,
                  luanChuyenVbUser.VB_QT_LuanChuyenVanBan.MaLenh, null, false,
                  qtLuanChuyenVb.FirstOrDefault().Id,
                  luanChuyenVbUser.VB_QT_LuanChuyenVanBan.MenuGuiId,
                  luanChuyenVbUser.VB_QT_LuanChuyenVanBan.MenuNhanId,
                  userNXL.PositionName, user.PositionName, userNXL.DepartmentName, user.DepartmentName);
                    _context.VB_QT_LuanChuyenVanBan.Add(lcvb);
                }
                if (luanChuyenVbUser.UserNhan.NguoiDXuLyId != null)
                {
                    VB_QT_LuanChuyenVanBan lcvb = LuanChuyenVanBan.r2AddLuanChuyenVanBan(
                  luanChuyenVbUser.VB_QT_LuanChuyenVanBan.VbMoiSoHoaId, luanChuyenVbUser.UserNhan.NguoiNDBId, luanChuyenVbUser.UserNhan.TenNguoiNDB, token.UserID, user.FullName,
                  luanChuyenVbUser.VB_QT_LuanChuyenVanBan.TieuDe,
                  luanChuyenVbUser.VB_QT_LuanChuyenVanBan.NoiDung, false,
                  luanChuyenVbUser.VB_QT_LuanChuyenVanBan.HanXuLy, null,
                  8,
                  luanChuyenVbUser.VB_QT_LuanChuyenVanBan.MaLenh, null, false,
                  qtLuanChuyenVb.FirstOrDefault().Id,
                  luanChuyenVbUser.VB_QT_LuanChuyenVanBan.MenuGuiId,
                  luanChuyenVbUser.VB_QT_LuanChuyenVanBan.MenuNhanId,
                  userNNDB.PositionName, user.PositionName, userNNDB.DepartmentName, user.DepartmentName);
                    _context.VB_QT_LuanChuyenVanBan.Add(lcvb);
                }
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0 });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { error = 1 });
            }


        }
        #endregion
    }
}