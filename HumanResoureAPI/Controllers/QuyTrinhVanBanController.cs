using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HumanResource.Application.Paremeters;
using HumanResource.Application.Paremeters.Dtos;
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
    public class QuyTrinhVanBanController : ControllerBase
    {
        private readonly humanDbContext _context;
        public QuyTrinhVanBanController(humanDbContext context)
        {
            _context = context;
        }
        #region Danh sách quy trình
        // Get: api/QuyTrinhVanBan/r1GetListDataQT
        [HttpGet]
        [Route("r1GetListDataQT")]
        public async Task<ActionResult<IEnumerable<VB_QT_QuyTrinh>>> r1GetListDataQT()
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                        var tables = _context.VB_QT_QuyTrinh.Select(a => new {
                            a.Name,
                            a.Id,
                            a.IsOrder
                        });
                        var qrs = await tables.OrderBy(x => x.IsOrder).ToListAsync();
                        return new ObjectResult(new { error = 0, data = qrs });
            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Danh sách bước
        // Post: api/QuyTrinhVanBan/r1GetListDataBuoc
        [HttpPost]
        [Route("r1GetListDataBuoc")]
        public async Task<ActionResult<IEnumerable<VB_QT_QuyTrinh>>> r1GetListDataBuoc(QuyTrinhVB options)
        {
            try
            {
                var buocLenhs =from b in _context.VB_QT_BuocLenhTuongTac
                                where b != null
                                join c in _context.VB_QT_LenhTuongTac on b.LenhTuongTacId equals c.Id
                                 select new
                                 {
                                     c.Name,
                                     c.Id,
                                     b.BuocId,
                                 };
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var tables = _context.VB_QT_Buoc.Where(x=>x.QuyTrinhId == options.QuyTrinhId && x.CompanyId == options.CompanyId).Select(a => new {
                    a.Name,
                    a.Id,
                    a.IsOrder,
                    children = buocLenhs.Where(x=>x.BuocId == a.Id).Select(a=> new
                    {
                        a.Name,
                        a.Id
                    }).ToList()
                }).AsQueryable();
                var qrs = await tables.OrderBy(x => x.IsOrder).ToListAsync();
                return new ObjectResult(new { error = 0, data = qrs });
            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Danh sách lệnh tương tác form quy trình
        // Post: api/QuyTrinhVanBan/r1GetListDataLTT
        [HttpPost]
        [Route("r1GetListDataLTT")]
        public async Task<ActionResult<IEnumerable<VB_QT_QuyTrinh>>> r1GetListDataLTT(QuyTrinhVB options)
        {
            try
            {
                var tables = _context.VB_QT_LenhTuongTac.Where(x=>x.QuyTrinhId == options.QuyTrinhId).Select(a => new {
                    a.Name,
                    a.Id,
                    a.Code,
                    a.IsOrder,
                    check = _context.VB_QT_BuocLenhTuongTac.Count(x=>x.LenhTuongTacId == a.Id && x.BuocId == options.BuocId) > 0 ? true : false
                }).Where(x=>x.check == false).AsQueryable();
                var qrs = await tables.OrderBy(x => x.IsOrder).ToListAsync();
                return new ObjectResult(new { error = 0, data = qrs });
            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Danh sách lệnh tương tác form cấu hình người nhận
        // Post: api/QuyTrinhVanBan/r1GetListDataLTT
        [HttpPost]
        [Route("r1GetListDataLTTCHNguoiNhan")]
        public async Task<ActionResult<IEnumerable<VB_QT_BuocLenhTuongTac>>> r1GetListDataLTTCHNguoiNhan(QuyTrinhVB options)
        {
            try
            {
                var tables = (from a in _context.VB_QT_BuocLenhTuongTac
                             where a != null
                             join b in _context.VB_QT_LenhTuongTac on a.LenhTuongTacId equals b.Id
                             where a.BuocId == options.BuocId
                             select new
                             {
                                 BuoclenhTTID = a.Id,
                                 b.Name,
                                 LenhTuongTacId = b.Id,
                                 b.IsActive,
                                 b.Code,
                                 b.IsOrder
                             }).AsQueryable();
                var qrs = await tables.OrderBy(x => x.IsOrder).ToListAsync();
                return new ObjectResult(new { error = 0, data = qrs });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Thêm lệnh tương tác
        // Post: api/QuyTrinhVanBan/r2AddListDataBuocLenh
        [HttpPost]
        [Route("r2AddListDataBuocLenh")]
        public async Task<ActionResult<IEnumerable<VB_QT_QuyTrinh>>> r2AddListDataBuocLenh(QuyTrinhVB options)
        {
            try
            {
                if (_context.VB_QT_BuocLenhTuongTac.Count(x=>x.LenhTuongTacId == options.LenhTuongTacId && x.BuocId == options.BuocId) > 0)
                {
                    return new ObjectResult(new { error = 2 });
                }
                VB_QT_BuocLenhTuongTac obj = new VB_QT_BuocLenhTuongTac();
                obj.BuocId = options.BuocId ?? 1;
                obj.LenhTuongTacId = options.LenhTuongTacId ?? 1;
                obj.IsOrder = _context.VB_QT_BuocLenhTuongTac.Count() + 1;
                _context.VB_QT_BuocLenhTuongTac.Add(obj);
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0});
            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Lấy cấu hình người nhận theo nhóm quyền
        // Post: api/QuyTrinhVanBan/r2GetCauHinhNN
        [HttpPost]
        [Route("r2GetCauHinhNN")]
        public async Task<ActionResult<IEnumerable<VB_QT_BuocLenhGroupRole>>> r2GetCauHinhNN(BuocLenhGroupRole options)
        {
            try
            {
                var tables = from a in _context.Sys_Dm_GroupRole
                             where a.CompanyId == options.CompanyId
                             select new
                             {
                                 a.Name,
                                 GroupRoleId = a.Id,
                                 a.IsOrder,
                                 IsDepartment = _context.VB_QT_BuocLenhGroupRole.Count(x => x.IsDepartment == true && x.GroupRoleId == a.Id && x.BuocLenhTuongTacId == options.BuocLenhTuongTacId) > 0 ? true : false,
                                 IsAll = _context.VB_QT_BuocLenhGroupRole.Count(x => x.IsAll == true && x.GroupRoleId == a.Id && x.BuocLenhTuongTacId == options.BuocLenhTuongTacId) > 0 ? true : false,
                                 IsNest = _context.VB_QT_BuocLenhGroupRole.Count(x => x.IsNest == true && x.GroupRoleId == a.Id && x.BuocLenhTuongTacId == options.BuocLenhTuongTacId) > 0 ? true : false,
                                 IsAllComCon = _context.VB_QT_BuocLenhGroupRole.Count(x => x.IsAllComCon == true && x.GroupRoleId == a.Id && x.BuocLenhTuongTacId == options.BuocLenhTuongTacId) > 0 ? true : false,
                                 IsAllComCha = _context.VB_QT_BuocLenhGroupRole.Count(x => x.IsAllComCha == true && x.GroupRoleId == a.Id && x.BuocLenhTuongTacId == options.BuocLenhTuongTacId) > 0 ? true : false,
                                 IsNguoiLap = _context.VB_QT_BuocLenhGroupRole.Count(x => x.IsNguoiLap == true && x.GroupRoleId == a.Id && x.BuocLenhTuongTacId == options.BuocLenhTuongTacId) > 0 ? true : false,
                                 IsNguoiGui = _context.VB_QT_BuocLenhGroupRole.Count(x => x.IsNguoiGui == true && x.GroupRoleId == a.Id && x.BuocLenhTuongTacId == options.BuocLenhTuongTacId) > 0 ? true : false
                             };
                var qrs = await tables.OrderBy(x => x.IsOrder).ToListAsync();
                return new ObjectResult(new { error = 0, data = qrs });
            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Thêm cấu hình người nhận theo nhóm quyền
        // Post: api/QuyTrinhVanBan/r2AddListDataNNToNhomQuyen
        [HttpPost]
        [Route("r2AddListDataNNToNhomQuyen")]
        public async Task<ActionResult<IEnumerable<VB_QT_QuyTrinh>>> r2AddListDataNNToNhomQuyen(BuocLenhGroupRole options)
        {
            try
            {
                if (_context.VB_QT_BuocLenhGroupRole.Count(x => x.GroupRoleId == options.GroupRoleId && x.BuocLenhTuongTacId == options.BuocLenhTuongTacId) > 0)
                {
                    var objup = _context.VB_QT_BuocLenhGroupRole.FirstOrDefault(x => x.GroupRoleId == options.GroupRoleId && x.BuocLenhTuongTacId == options.BuocLenhTuongTacId);
                    objup.IsAll = options.IsAll?? false;
                    objup.IsDepartment = options.IsDepartment ?? false;
                    objup.IsNest = options.IsNest ?? false;
                    objup.IsAllComCha = options.IsAllComCha ?? false;
                    objup.IsAllComCon = options.IsAllComCon ?? false;
                    objup.IsNguoiGui = options.IsNguoiGui ?? false;
                    objup.IsNguoiLap = options.IsNguoiLap ?? false;
                } else
                {
                    VB_QT_BuocLenhGroupRole obj = new VB_QT_BuocLenhGroupRole();
                    obj.BuocLenhTuongTacId = options.BuocLenhTuongTacId;
                    obj.GroupRoleId = options.GroupRoleId;
                    obj.IsAll = options.IsAll ?? false;
                    obj.IsDepartment = options.IsDepartment ?? false;
                    obj.IsNest = options.IsNest ?? false;
                    obj.IsAllComCha = options.IsAllComCha ?? false;
                    obj.IsAllComCon = options.IsAllComCon ?? false;
                    obj.IsNguoiGui = options.IsNguoiGui ?? false;
                    obj.IsNguoiLap = options.IsNguoiLap ?? false;
                    _context.VB_QT_BuocLenhGroupRole.Add(obj);
                }
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0 });
            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Xóa lệnh tương tác khỏi bước
        // Post: api/QuyTrinhVanBan/r4DelListDataBuocLenh
        [HttpPost]
        [Route("r4DelListDataBuocLenh")]
        public async Task<ActionResult<IEnumerable<VB_QT_QuyTrinh>>> r4DelListDataBuocLenh(QuyTrinhVB options)
        {
            try
            {
                var obj = _context.VB_QT_BuocLenhTuongTac.FirstOrDefault(x => x.LenhTuongTacId == options.LenhTuongTacId && x.BuocId == options.BuocId);
                _context.VB_QT_BuocLenhTuongTac.Remove(obj);
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0 });
            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
    }
}