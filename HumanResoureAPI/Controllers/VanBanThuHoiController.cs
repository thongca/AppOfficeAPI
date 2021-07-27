using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HumanResource.Application.Helper.Dtos;
using HumanResource.Data.EF;
using HumanResource.Data.Entities.VanBan;
using HumanResoureAPI.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HumanResoureAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VanBanThuHoiController : ControllerBase
    {
        private readonly humanDbContext _context;
        public VanBanThuHoiController(humanDbContext context)
        {
            _context = context;
        }
        #region Danh sách văn bản mới số hóa
        // Post: api/VanBanThuHoi/r2ThuHoiQuyTrinhVanBan
        [HttpPost]
        [Route("r2ThuHoiQuyTrinhVanBan")]
        public async Task<ActionResult<IEnumerable<VB_QT_LuanChuyenVanBan>>> r2ThuHoiQuyTrinhVanBan(VB_QT_LuanChuyenVanBan vB_QT_LuanChuyenVanBan)
        {
             RequestToken token = CommonData.GetDataFromToken(User);
            var tables = _context.VB_QT_LuanChuyenVanBan.Where(b => b.MaLenh == "VB_XNHT" && b.NguoiGuiId == token.UserID && b.VbMoiSoHoaId == vB_QT_LuanChuyenVanBan.VbMoiSoHoaId);
            if (tables == null)
            {
                return new ObjectResult(new { error = 1 });
            }
            foreach (var item in tables)
            {
                item.MaLenh = "VB_THUHOI";
                item.NgayXuLy = DateTime.Now;
                item.NoiDung =vB_QT_LuanChuyenVanBan.NoiDung;
            }
           await _context.SaveChangesAsync();
            return new ObjectResult(new { error = 0, });

        }
        #endregion
    }
}