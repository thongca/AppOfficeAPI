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
    public class VanBanBiTraLaiController : ControllerBase
    {
        private readonly humanDbContext _context;
        public VanBanBiTraLaiController(humanDbContext context)
        {
            _context = context;

        }
        #region Danh sách văn bản bị trả lại
        // Get: api/VanBanBiTraLai/r1GetListVBBiTraLai
        [HttpGet]
        [Route("r1GetListVBBiTraLai")]
        public async Task<ActionResult<IEnumerable<VB_QT_VanBanMoiSoHoa>>> r1GetListVBBiTraLai()
        {
             RequestToken token = CommonData.GetDataFromToken(User);
            var tables = from b in _context.VB_QT_LuanChuyenVanBan
                         join a in _context.VB_QT_VanBanMoiSoHoa on b.VbMoiSoHoaId equals a.Id
                         where b.MaLenh == "VB_TRALAI" && b.NgayXuLy == null && b.NguoiNhanId == token.UserID
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