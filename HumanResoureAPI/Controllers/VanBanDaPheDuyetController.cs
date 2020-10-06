using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HumanResource.Data.EF;
using HumanResource.Data.Entities.VanBan;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HumanResoureAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VanBanDaPheDuyetController : ControllerBase
    {
        private readonly humanDbContext _context;
        public VanBanDaPheDuyetController(humanDbContext context)
        {
            _context = context;

        }
        #region Danh sách văn bản đã phê duyệt
        // Get: api/VanBanDaPheDuyet/r1GetListVBDaPheDuyet
        [HttpGet]
        [Route("r1GetListVBDaPheDuyet")]
        public async Task<ActionResult<IEnumerable<VB_QT_VanBanMoiSoHoa>>> r1GetListVBDaPheDuyet()
        {
            var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
            var tables = from b in _context.VB_QT_LuanChuyenVanBan
                         join a in _context.VB_QT_VanBanMoiSoHoa on b.VbMoiSoHoaId equals a.Id
                         where b.MaLenh == "VB_DAPHEDUYET" && b.NgayXuLy == null && b.NguoiNhanId == userId
                         orderby b.ThoiGianGui descending
                         select new
                         {
                             b.MaLenh,
                             b.TenNguoiGui,
                             b.Id,
                             b.VbMoiSoHoaId,
                             a.TrichYeu,
                             a.NgayBanHanh,
                             a.SoKyHieu,
                             a.CreateDate
                         };
            return new ObjectResult(new { error = 0, data = await tables.OrderBy(x => x.VbMoiSoHoaId).ToListAsync() });

        }
        #endregion
        #region Danh sách văn bản mình đã phê duyệt
        // Get: api/VanBanDaPheDuyet/r1GetListVBMinhDaPheDuyet
        [HttpGet]
        [Route("r1GetListVBMinhDaPheDuyet")]
        public async Task<ActionResult<IEnumerable<VB_QT_VanBanMoiSoHoa>>> r1GetListVBMinhDaPheDuyet()
        {
            var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
            var tables = (from b in _context.VB_QT_LuanChuyenVanBan
                         join a in _context.VB_QT_VanBanMoiSoHoa on b.VbMoiSoHoaId equals a.Id
                         where b.MaLenh == "VB_DAPHEDUYET" && b.NguoiGuiId == userId

                         select new
                         {
                             b.MaLenh,
                             _context.VB_QT_LuanChuyenVanBan.Where(x=>x.Id == b.ParentId && x.NguoiNhanId == userId && x.VbMoiSoHoaId == b.VbMoiSoHoaId).FirstOrDefault().TenNguoiGui,
                             b.VbMoiSoHoaId,
                             a.TrichYeu,
                             a.NgayBanHanh,
                             a.SoKyHieu,
                             a.CreateDate
                         }).Distinct();
            return new ObjectResult(new { error = 0, data = await tables.OrderBy(x => x.VbMoiSoHoaId).ToListAsync() });

        }
        #region lay 1 văn bản bằng Id
        // Get: api/VanBanDaPheDuyet
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<VB_QT_VanBanMoiSoHoa>>> r1GetVanBanSoHoaById(string id)
        {
            var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
            var tables = await _context.VB_QT_VanBanMoiSoHoa.FindAsync(id);
            var vb = await (from a in _context.VB_QT_VanBanMoiSoHoa
                            join b in _context.VB_Dm_LinhVuc on a.LinhVucId equals b.Id
                            join c in _context.VB_Dm_LoaiVanBan on a.LoaiVanBanId equals c.Id
                            where a.Id == id
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
            var files = _context.VB_QT_FileVBMoiSoHoa.Where(x => x.VbMoiSoHoaId == id).Select(c => new
            {
                c.Name,
                c.Path,
                c.Size
            }).ToList();
            var lcvbs = _context.VB_QT_LuanChuyenVanBan.Where(x => x.VbMoiSoHoaId == id).Select(c => new
            {
                c.Id,
                c.TenNguoiNhan,
                c.TenNguoiGui,
                c.ThoiGianGui,
                c.TrangThaiXuLy,
                c.NoiDung,
                c.NgayDoc,
                c.NgayXuLy,
                c.ParentId
            }).OrderByDescending(x => x.ThoiGianGui).ToList();
            return new ObjectResult(new { error = 0, data = vb[0], files, lcvbs });
        }
        #endregion
        #endregion
    }
}