using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using HumanResource.Application.Paremeters;
using HumanResource.Data.EF;
using HumanResource.Data.Entities.VanBan;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HumanResoureAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VanBanCommonController : ControllerBase
    {
        private readonly humanDbContext _context;
        public VanBanCommonController(humanDbContext context)
        {
            _context = context;
        }
        #region Danh sách lĩnh vực
        // Get: api/VanBanCommon/r1GetListLinhVuc
        [HttpGet]
        [Route("r1GetListLinhVuc")]
        public async Task<ActionResult<IEnumerable<VB_Dm_LinhVuc>>> r1GetListLinhVuc()
        {
            var tables = _context.VB_Dm_LinhVuc.Select(a => new
            {
                a.Name,
                a.Id,
            });
            return new ObjectResult(new { error = 0, data = await tables.OrderBy(x => x.Id).ToListAsync() });

        }
        #endregion
        #region Danh sách loại văn bản
        // Get: api/VanBanCommon/r1GetListLoaiVanBan
        [HttpGet]
        [Route("r1GetListLoaiVanBan")]
        public async Task<ActionResult<IEnumerable<VB_Dm_LinhVuc>>> r1GetListLoaiVanBan()
        {
            var tables = _context.VB_Dm_LoaiVanBan.Select(a => new
            {
                a.Name,
                a.Id,
            });
            return new ObjectResult(new { error = 0, data = await tables.OrderBy(x => x.Id).ToListAsync() });

        }
        #endregion
        #region Danh sách nhân sự
        // Get: api/VanBanCommon/r1GetListNhanSu
        [HttpGet]
        [Route("r1GetListNhanSu")]
        public async Task<ActionResult<IEnumerable<VB_Dm_LinhVuc>>> r1GetListNhanSu()
        {
            var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
            var user = await _context.Sys_Dm_User.FindAsync(userId);
            var tables = _context.Sys_Dm_User.Where(x => x.CompanyId == user.CompanyId).Select(a => new
            {
                a.FullName,
                a.Id,
            });
            return new ObjectResult(new { error = 0, data = await tables.OrderBy(x => x.Id).ToListAsync() });

        }
        #endregion
        #region kiểm tra người nhận lúc xác nhận hoàn thành
        // Get: api/VanBanCommon/r1checkNguoiNhanXNHT
        [HttpPost]
        [Route("r1checkNguoiNhanXNHT")]
        public async Task<ActionResult<IEnumerable<VB_QT_BuocLenhGroupRole>>> r1checkNguoiNhanXNHT(NguoiNhanXNHT op)
        {
            var tables =await _context.VB_QT_BuocLenhGroupRole.FindAsync(op.Id);
            return new ObjectResult(new { error = 0, tables.IsNguoiGui, tables.IsNguoiLap });

        }
        #endregion
        #region Danh sách văn bản nhận thông báo
        // Get: api/VanBanCommon/r1GetListVBNhanThongBao
        [HttpGet]
        [Route("r1GetListVBNhanThongBao")]
        public async Task<ActionResult<IEnumerable<VB_QT_VanBanMoiSoHoa>>> r1GetListVBNhanThongBao()
        {
            var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
            var tables = from b in _context.VB_QT_LuanChuyenVanBan
                         join a in _context.VB_QT_VanBanMoiSoHoa on b.VbMoiSoHoaId equals a.Id
                         where b.MaLenh == "VB_NHANTHONGBAO" && b.NgayXuLy == null && b.NguoiNhanId == userId
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
    }
}