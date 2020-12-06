using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HumanResource.Application.Paremeters;
using HumanResource.Application.Paremeters.Dtos;
using HumanResource.Data.EF;
using HumanResource.Data.Entities.System;
using HumanResource.Data.Entities.VanBan;
using HumanResoureAPI.Common;
using HumanResoureAPI.Common.Systems;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HumanResoureAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private readonly humanDbContext _context;
        public CommonController(humanDbContext context)
        {
            _context = context;
        }
        #region Danh sách công ty
        // Post: api/Common/r1GetListCompany
        [HttpPost]
        [Route("r1GetListCompany")]
        public async Task<ActionResult<IEnumerable<Sys_Dm_Company>>> r1GetListData(Options options)
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                int groupRoleDeFault = CheckPermission.getGroupRoleDefault(_context, userId);
                int perMission = CheckPermission.CheckPer(_context, userId, options.groupId);
                switch (perMission)
                {
                    case 0:
                        var tables = _context.Sys_Dm_Company.Select(a => new
                        {
                            Name = a.Code + " " + a.Name,
                            a.Id,
                            a.IsOrder
                        });
                        var qrs = await tables.OrderBy(x => x.IsOrder).ToListAsync();
                        return new ObjectResult(new { error = 0, data = qrs });
                    default:
                        return new ObjectResult(new { error = 0, data = new List<Sys_Dm_Company>() });
                }

            }
            catch (Exception e)
            {
                bool success = SaveLog.SaveLogEx(_context, "api/Common/r1GetListCompany", e.Message, "Danh sách công ty");
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Danh sách phòng ban
        // Post: api/Common/r1GetListDataCommonDep
        [HttpPost]
        [Route("r1GetListDataCommonDep")]
        public async Task<ActionResult<IEnumerable<Sys_Dm_Company>>> r1GetListDataDepartment(Options options)
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var tables = _context.Sys_Dm_Department.Where(x => x.ParentId == null && x.CompanyId == options.companyId).Select(a => new
                {
                    Name = "(" + a.Code + ") " + a.Name,
                    a.Id,
                    a.IsOrder
                });
                var qrs = await tables.OrderBy(x => x.IsOrder).ToListAsync();
                return new ObjectResult(new { error = 0, data = qrs });

            }
            catch (Exception e)
            {
                bool success = SaveLog.SaveLogEx(_context, "api/Common/r1GetListDataDepartment", e.Message, "Danh sách phòng ban");
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Danh sách phòng ban theo công ty của user login
        // GET: api/Common/r1GetListDataDepforUser
        [HttpGet]
        [Route("r1GetListDataDepforUser")]
        public async Task<ActionResult<IEnumerable<Sys_Dm_Company>>> r1GetListDataDepforUser()
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var user = await _context.Sys_Dm_User.FindAsync(userId);
                var tables = _context.Sys_Dm_Department.Where(x => x.ParentId == null && x.CompanyId == user.CompanyId).Select(a => new
                {
                    DepartmentName = "(" + a.Code + ") " + a.Name,
                    DepartmentId = a.Id,
                    a.IsOrder
                });
                var qrs = await tables.OrderBy(x => x.IsOrder).ToListAsync();
                return new ObjectResult(new { error = 0, data = qrs });

            }
            catch (Exception e)
            {
                bool success = SaveLog.SaveLogEx(_context, "api/Common/r1GetListDataDepartment", e.Message, "Danh sách phòng ban");
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Danh sách chức vụ
        // Post: api/Common/r1GetListDataPosition
        [HttpPost]
        [Route("r1GetListDataPosition")]
        public async Task<ActionResult<IEnumerable<Sys_Dm_Company>>> r1GetListDataPosition(Options options)
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var tables = _context.Sys_Dm_Position.Where(x => x.CompanyId == options.companyId).Select(a => new
                {
                    a.Name,
                    a.Id
                });
                var qrs = await tables.OrderBy(x => x.Id).ToListAsync();
                return new ObjectResult(new { error = 0, data = qrs });

            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }
        }

        #endregion
        #region Danh sách tổ
        // Post: api/Common/r1GetListDataNest
        [HttpPost]
        [Route("r1GetListDataNest")]
        public async Task<ActionResult<IEnumerable<Sys_Dm_Company>>> r1GetListDataNest(Options options)
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var tables = _context.Sys_Dm_Department.Where(x => x.ParentId == options.departmentId && x.CompanyId == options.companyId).Select(a => new
                {
                    Name = "(" + a.Code + ") " + a.Name,
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
        #region Danh sách nhóm quyền
        // Post: api/Common/r1GetListGroupRole
        [HttpPost]
        [Route("r1GetListGroupRole")]
        public async Task<ActionResult<IEnumerable<Sys_Dm_GroupRole>>> r1GetListGroupRole(Options options)
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var tables = _context.Sys_Dm_GroupRole
                    .Where(x =>
                    x.CompanyId == options.companyId &&
                    x.RankRole > options.rankrole &&
                    x.IsAdministrator != true
                ).Select(a => new
                {
                    a.Name,
                    a.Id,
                    a.IsOrder,
                    a.CompanyId,
                    a.DepartmentId
                });
                if (options.companyId == 0)
                {
                    tables = tables.Where(x => x.CompanyId == 0);
                }
                if (options.companyId > 0 && options.departmentId == 0 && options.nestId == 0)
                {
                    tables = tables.Where(x => x.CompanyId == options.companyId && x.DepartmentId == 0);
                }
                if (options.departmentId > 0 && options.nestId == 0)
                {
                    tables = tables.Where(x => x.DepartmentId == options.departmentId);
                }
                if (options.nestId > 0)
                {
                    tables = tables.Where(x => x.DepartmentId == options.nestId);
                }
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
        // Post: api/Common/r1GetListDataBuoc
        [HttpPost]
        [Route("r1GetListDataBuoc")]
        public async Task<ActionResult<IEnumerable<VB_QT_Buoc>>> r1GetListDataBuoc(QuyTrinhVB options)
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var tables = _context.VB_QT_Buoc.Where(x => x.QuyTrinhId == options.QuyTrinhId && x.CompanyId == options.CompanyId).Select(a => new
                {
                    a.Name,
                    a.Id,
                    a.IsOrder
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
        #region Danh sách lệnh chọn
        // Post: api/Common/r1GetListDataLenhTheoUser
        [HttpPost]
        [Route("r1GetListDataLenhTheoUser")]
        public async Task<ActionResult<IEnumerable<VB_QT_Buoc>>> r1GetListDataLenhTheoUser(LenhMenuForUser options)
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var user = await _context.Sys_Dm_User.FindAsync(userId);
                var tables = from a in _context.VB_QT_BuocLenhGroupRole
                             where a != null
                             join b in _context.VB_QT_BuocLenhTuongTac on a.BuocLenhTuongTacId equals b.Id
                             join c in _context.VB_QT_LenhTuongTac on b.LenhTuongTacId equals c.Id
                             join d in _context.VB_QT_Buoc on b.BuocId equals d.Id
                             where a.GroupRoleId == options.GroupRoleId && d.MenuId == options.MenuId
                             select new
                             {
                                 c.Name,
                                 BuocLenhGroupId = a.Id,
                                 c.IsActive,
                                 c.IsOrder,
                                 c.Code

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
        #region mô hình tổ chức
        // Post: api/Common/r1GetListDataMohinhToChuc
        [HttpPost]
        [Route("r1GetListDataMohinhToChuc")]
        public async Task<ActionResult<IEnumerable<VB_QT_Buoc>>> r1GetListDataMohinhToChuc(BuocLenhGroupForUser options)
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var user = await _context.Sys_Dm_User.FindAsync(userId);
                int hienNguoiNhan = CheckNguoiNhan.DuocHienThiNguoiNhan(_context, options.GroupRoleId, options.BuocLenhGroupId);
                switch (hienNguoiNhan)
                {
                    #region Toàn công ty
                    case 0:
                        var _listUniOn = await _context.Sys_Dm_Department.Where(x => x.ParentId == null && x.CompanyId == 1).Select(a => new
                        {
                            a.Id,
                            a.Name,
                            ParentId = a.CompanyId,
                            Loai = 1
                        }).Union(
              _context.Sys_Dm_Company.Where(x => x.ParentId == 1).Select(c => new
              {
                  c.Id,
                  c.Name,
                  ParentId = 1,
                  Loai = 0
              })).ToListAsync();

                        var _listDepartMenttct = _context.Sys_Dm_Department.Where(x => x.ParentId == null).Select(a => new
                        {
                            a.Id,
                            a.Name,
                            a.CompanyId,
                            Loai = 1,
                            children = _context.Sys_Dm_Department.Where(x => x.ParentId == a.Id).Select(v => new
                            {
                                v.Id,
                                v.Name,
                                Loai = 2
                            }).ToList()
                        });
                        var s = _listUniOn.Select(x => new
                        {
                            x.Id,
                            x.Name,
                            x.ParentId,
                            x.Loai,
                            children = x.Loai == 0 ? _listDepartMenttct.Where(b => b.CompanyId == x.Id).Select(k => new
                            {
                                k.Id,
                                k.Name,
                                k.Loai,
                                k.children
                            }).ToList() : _listDepartMenttct.Where(b => b.CompanyId == 0).Select(k => new
                            {
                                k.Id,
                                k.Name,
                                k.Loai,
                                k.children
                            }).ToList()
                        });
                        var tables = (from a in _context.Sys_Dm_Company
                                      where a.ParentId == null
                                      select new
                                      {
                                          a.Id,
                                          a.Name,
                                          Loai = 0,
                                          a.IsOrder,
                                          children = s.ToList()
                                      }).ToList();
                        return new ObjectResult(new { error = 0, data = tables.OrderBy(x => x.IsOrder) });
                    #endregion
                    #region Công ty mẹ
                    case 1:

                        var _listDepartMentctm = _context.Sys_Dm_Department.Where(x => x.ParentId == null).Select(a => new
                        {
                            a.Id,
                            a.Name,
                            a.CompanyId,
                            Loai = 1,
                            children = _context.Sys_Dm_Department.Where(x => x.ParentId == a.Id).Select(v => new
                            {
                                v.Id,
                                v.Name,
                                Loai = 2
                            }).ToList()
                        });

                        var tables2 = (from a in _context.Sys_Dm_Company
                                       where a.ParentId == null
                                       select new
                                       {
                                           a.Id,
                                           a.Name,
                                           Loai = 0,
                                           children = _listDepartMentctm.Where(x => x.CompanyId == a.Id).Select(c => new
                                           {
                                               c.Id,
                                               c.Name,
                                               c.Loai,
                                               c.children
                                           }).ToList()
                                       }).ToList();
                        return new ObjectResult(new { error = 0, data = tables2 });
                    #endregion
                    #region Công ty hiện tại
                    case 2:

                        var _listDepartMentctc = _context.Sys_Dm_Department.Where(x => x.ParentId == null).Select(a => new
                        {
                            a.Id,
                            a.Name,
                            a.CompanyId,
                            Loai = 1,
                            children = _context.Sys_Dm_Department.Where(x => x.ParentId == a.Id).Select(v => new
                            {
                                v.Id,
                                v.Name,
                                Loai = 2
                            }).ToList()
                        });

                        var tables3 = (from a in _context.Sys_Dm_Company
                                       where a.Id == user.CompanyId
                                       select new
                                       {
                                           a.Id,
                                           a.Name,
                                           Loai = 0,
                                           children = _listDepartMentctc.Where(x => x.CompanyId == user.CompanyId).Select(c => new
                                           {
                                               c.Id,
                                               c.Name,
                                               c.Loai,
                                               c.children
                                           }).ToList()
                                       }).ToList();
                        return new ObjectResult(new { error = 0, data = tables3 });
                    #endregion
                    #region Phòng ban 
                    case 3:
                        int DepId = 0;
                        var room = await _context.Sys_Dm_Department.FindAsync(user.DepartmentId);
                        if (room.ParentId == null)
                        {
                            DepId = room.Id;
                        }
                        else
                        {
                            DepId = room.ParentId ?? 0;
                        }
                        var _listDepartMents = _context.Sys_Dm_Department.Where(x => x.CompanyId == user.CompanyId && x.Id == DepId).Select(a => new
                        {
                            a.Id,
                            a.Name,
                            a.CompanyId,
                            Loai = 1,
                            children = _context.Sys_Dm_Department.Where(x => x.ParentId == a.Id).Select(v => new
                            {
                                v.Id,
                                v.Name,
                                Loai = 2
                            }).ToList()
                        });
                        return new ObjectResult(new { error = 0, data = _listDepartMents });
                    #endregion
                    #region Tổ
                    case 4:
                        var _listDepartMentTo = (_context.Sys_Dm_Department.Where(x => x.CompanyId == user.CompanyId && x.Id == user.DepartmentId).Select(a => new
                        {
                            a.Id,
                            a.Name,
                            a.CompanyId,
                            Loai = 2,
                            children = _context.Sys_Dm_Department.Where(x => x.ParentId == a.Id).Select(v => new
                            {
                                v.Id,
                                v.Name,
                                Loai = 2
                            }).ToList()
                        })).ToList();
                        return new ObjectResult(new { error = 0, data = _listDepartMentTo });
                    #endregion
                    default:
                        var _listDepartMentToe = (_context.Sys_Dm_Department.Where(x => x.CompanyId == user.CompanyId && x.Id == user.DepartmentId).Select(a => new
                        {
                            a.Id,
                            a.Name,
                            a.CompanyId,
                            Loai = 2,
                            children = _context.Sys_Dm_Department.Where(x => x.ParentId == a.Id).Select(v => new
                            {
                                v.Id,
                                v.Name,
                                Loai = 2
                            }).ToList()
                        })).ToList();
                        return new ObjectResult(new { error = 0, data = _listDepartMentToe });
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Danh sách nhân viên trong phòng
        // Post: api/Common/r1GetListDataUserForDepartment
        [HttpPost]
        [Route("r1GetListDataUserForDepartment")]
        public async Task<ActionResult<IEnumerable<VB_QT_Buoc>>> r1GetListDataUserForDepartment(BuocLenhGroupForUser options)
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var user = await _context.Sys_Dm_User.FindAsync(userId);
                #region Phòng ban 

                int DepId = 0;
                var room = await _context.Sys_Dm_Department.FindAsync(user.DepartmentId);
                if (room.ParentId == null)
                {
                    DepId = room.Id;
                }
                else
                {
                    DepId = room.ParentId ?? 0;
                }
                var listPb = _context.Sys_Dm_Department.Where(x => x.ParentId == DepId).Select(c => c.Id);
                var listNsPB = await _context.Sys_Dm_User.Where(x => listPb.Contains(x.DepartmentId ?? 0) || x.DepartmentId == DepId).Select(a => new
                {
                    UserId = a.Id,
                    a.FullName
                }).ToListAsync();
                return new ObjectResult(new { error = 0, data = listNsPB });
                #endregion
            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region mô hình tổ chức giao việc
        // Post: api/Common/r1GetListUserNhanViec
        [HttpPost]
        [Route("r1GetListUserNhanViec")]
        public async Task<ActionResult<IEnumerable<VB_QT_Buoc>>> r1GetListUserNhanViec(BuocLenhGroupForUser options)
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var user = await _context.Sys_Dm_User.FindAsync(userId);
                var buoc = _context.VB_QT_Buoc.FirstOrDefault(x => x.MenuId == options.MenuId);
                var LenhTuongTac = _context.VB_QT_LenhTuongTac.FirstOrDefault(x => x.Code == options.MaLenh);
                var buocLenhTuongTac = _context.VB_QT_BuocLenhTuongTac.FirstOrDefault(x => x.BuocId == buoc.Id && x.LenhTuongTacId == LenhTuongTac.Id);
                int hienNguoiNhan = 0;
                if (buocLenhTuongTac != null)
                {
                    var buocLenhGroup = _context.VB_QT_BuocLenhGroupRole.FirstOrDefault(x => x.GroupRoleId == options.GroupRoleId && x.BuocLenhTuongTacId == buocLenhTuongTac.Id);
                    hienNguoiNhan = CheckNguoiNhan.DuocHienThiNguoiNhan(_context, options.GroupRoleId, buocLenhGroup.Id);
                }

                switch (hienNguoiNhan)
                {
                    #region Toàn công ty
                    case 0:
                        var listNsAll = await _context.Sys_Dm_User.Select(a => new
                        {
                            UserId = a.Id,
                            a.FullName
                        }).ToListAsync();
                        return new ObjectResult(new { error = 0, data = listNsAll });
                    #endregion
                    #region Công ty mẹ
                    case 1:
                        var listNsCTM = await _context.Sys_Dm_User.Where(x => x.CompanyId == 1).Select(a => new
                        {
                            UserId = a.Id,
                            a.FullName
                        }).ToListAsync();

                        return new ObjectResult(new { error = 0, data = listNsCTM });
                    #endregion
                    #region Công ty hiện tại
                    case 2:

                        var listNsCurrent = await _context.Sys_Dm_User.Where(x => x.CompanyId == user.CompanyId).Select(a => new
                        {
                            UserId = a.Id,
                            a.FullName
                        }).ToListAsync();
                        return new ObjectResult(new { error = 0, data = listNsCurrent });
                    #endregion
                    #region Phòng ban 
                    case 3:
                        int DepId = 0;
                        var room = await _context.Sys_Dm_Department.FindAsync(user.DepartmentId);
                        if (room.ParentId == null)
                        {
                            DepId = room.Id;
                        }
                        else
                        {
                            DepId = room.ParentId ?? 0;
                        }
                        var listPb = _context.Sys_Dm_Department.Where(x => x.ParentId == DepId).Select(c => c.Id);
                        var listNsPB = await _context.Sys_Dm_User.Where(x => listPb.Contains(x.DepartmentId ?? 0) || x.DepartmentId == DepId).Select(a => new
                        {
                            UserId = a.Id,
                            a.FullName
                        }).ToListAsync();
                        return new ObjectResult(new { error = 0, data = listNsPB });
                    #endregion
                    #region Tổ
                    case 4:
                        var listNsTo = await _context.Sys_Dm_User.Where(x => x.DepartmentId == user.DepartmentId).Select(a => new
                        {
                            UserId = a.Id,
                            a.FullName
                        }).ToListAsync();
                        return new ObjectResult(new { error = 0, data = listNsTo });
                    #endregion
                    #region Chỉ trưởng phòng
                    case 7:
                        var listNsInPB = await _context.Sys_Dm_User.Where(x => x.ParentDepartId == user.ParentDepartId).Select(a => a.Id).ToListAsync();
                        var tps = await (from b in _context.Sys_Cog_UsersGroup
                                         join c in _context.Sys_Dm_GroupRole on b.GroupRoleId equals c.Id
                                         join a in _context.Sys_Dm_User on b.UserId equals a.Id
                                         where listNsInPB.Contains(b.UserId) && c.IsAdminDep == true
                                         select new
                                         {
                                             b.UserId,
                                             a.FullName
                                         }).ToListAsync();

                        return new ObjectResult(new { error = 0, data = tps });
                    #endregion
                    default:
                        var listNsDef = await _context.Sys_Dm_User.Where(x => x.DepartmentId == user.DepartmentId).Select(a => new
                        {
                            UserId = a.Id,
                            a.FullName
                        }).ToListAsync();
                        return new ObjectResult(new { error = 0, data = listNsDef });
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Danh sách người dùng
        // Post: api/Common/r1GetListUser
        [HttpPost]
        [Route("r1GetListUser")]
        public async Task<ActionResult<IEnumerable<Sys_Dm_User>>> r1GetListUser(OptionSelectUser options)
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var tables = _context.Sys_Dm_User.Select(a => new
                {
                    a.FullName,
                    a.Id,
                    a.DepartmentId,
                    a.CompanyId
                }).AsQueryable();

                if (options.companyId > 0)
                {
                    tables = tables.Where(x => x.CompanyId == options.companyId);
                }
                if (options.departmentId > 0)
                {
                    var depart = _context.Sys_Dm_Department.Where(x => x.ParentId == options.departmentId).Select(c => c.Id);
                    tables = tables.Where(x => x.DepartmentId == options.departmentId || depart.Contains(x.DepartmentId ?? 0));
                }
                if (options.nestId > 0)
                {
                    tables = tables.Where(x => x.DepartmentId == options.nestId);
                }

                if (!string.IsNullOrEmpty(options.s))
                {
                    tables = tables.Where(c => c.FullName.ToUpper().Contains(options.s.ToUpper()));
                }
                var qrs = await tables.OrderBy(x => x.Id).ToListAsync();
                return new ObjectResult(new { error = 0, data = qrs, total = tables.Count() });

            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Thông báo
        // Get: api/Common/r1GetListThongBao
        [HttpGet]
        [Route("r1GetListThongBao")]
        public async Task<ActionResult<IEnumerable<Sys_Dm_Company>>> r1GetListThongBao()
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var tables = _context.Sys_QT_ThongBao.Where(x => x.NguoiNhanId == userId).Select(a => new
                {
                    a.TenNguoiGui,
                    a.NoiDung,
                    a.Id,
                    a.TrangThai,
                    a.TrangThaiXuLy,
                    a.NgayGui,
                    DaXem = a.DaDoc == true ? "Đã xem" : "",
                    a.DaDoc,
                    a.RouterLink
                });
                var qrs = await tables.OrderByDescending(x => x.NgayGui).Take(5).ToListAsync();
                return new ObjectResult(new { error = 0, data = qrs, total = tables.Count(x => x.DaDoc != true) });

            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }
        }
        // Post: api/Common/r1PostUpdateThongBao
        [HttpPost]
        [Route("r1PostUpdateThongBao")]
        public async Task<ActionResult<IEnumerable<Sys_Dm_Company>>> r1PostUpdateThongBao(OptionId thongbao)
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var tables = await _context.Sys_QT_ThongBao.FindAsync(thongbao.Id);
                tables.DaDoc = true;
                tables.NgayDoc = DateTime.Now;
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0 });

            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }
        }
        // Get: api/Common/r1GetUpdateAllThongBao
        [HttpGet]
        [Route("r1GetUpdateAllThongBao")]
        public async Task<ActionResult<IEnumerable<Sys_Dm_Company>>> r1GetUpdateAllThongBao()
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var tables = await _context.Sys_QT_ThongBao.Where(x => x.NguoiNhanId == userId).ToListAsync();
                foreach (var item in tables)
                {

                    item.DaDoc = true;
                    item.NgayDoc = DateTime.Now;
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

        private static string getTrangThaiXuLy(int ttxl, int QuytrinhId)
        {
            if (QuytrinhId == 1)
            {
                switch (ttxl)
                {
                    case 1:
                        return "Văn bản mới số hóa";
                    case 2:
                        return "Văn bản chờ phê duyệt";
                    case 3:
                        return "Văn bản đã phê duyệt";
                    case 4:
                        return "Văn bản bị trả lại";
                    case 5:
                        return "Văn bản nhận thông báo";
                    case 6:
                        return "Văn bản xử lý chính";
                    case 7:
                        return "Văn bản đồng xử lý";
                    case 8:
                        return "Văn bản nhận để biết";
                    default:
                        return "Văn bản mới số hóa";
                }
            }
            else
            {
                switch (ttxl)
                {
                    case 1:
                        return "Trình phê duyệt thời hạn";
                    case 2:
                        return "Đã phê duyệt thời hạn";
                    case 3:
                        return "Đã phê duyệt thời hạn";
                    case 4:
                        return "Trình phê duyệt kết quả";
                    case 5:
                        return "Yêu cầu chỉnh sửa";
                    case 6:
                        return "Đạt chất lượng";
                    case 7:
                        return "Chỉnh sửa phát sinh";
                    case 8:
                        return "Duyệt chỉnh sửa phát sinh";
                    case 9:
                        return "Bị đánh giá CLCV";
                    case 10:
                        return "Nhắc nhở và gia hạn";
                    case 11:
                        return "Chuyển công việc";
                    case 12:
                        return "Chuyển kết quả cc";
                    default:
                        return "Công việc mới";
                }
            }

        }
    }
}