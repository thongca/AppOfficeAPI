﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HumanResource.Application.Helper;
using HumanResource.Application.Helper.Dtos;
using HumanResource.Application.Paremeters;
using HumanResource.Data.EF;
using HumanResource.Data.Entities.System;
using HumanResoureAPI.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HumanResoureAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly humanDbContext _context;
        private readonly onlineDbContext _onlinecontext;
        private readonly IAuthentication _authentication;
        public LoginController(humanDbContext context, IAuthentication authentication, onlineDbContext onlinecontext )
        {
            _context = context;
            _authentication = authentication;
            _onlinecontext = onlinecontext;
        }
        // GET: api/Login
        [HttpPost]
        [Route("CheckLogin")]
        public async Task<ActionResult<IEnumerable<Sys_Dm_User>>> CheckLogin([FromBody] CheckLogin checklogin)
        {

            try
            {
                
                string PasswordEn = Helper.Encrypt(checklogin.UserName, checklogin.Password);
                var useronline = _onlinecontext.Sys_Dm_Lisesion.Count(x => x.Login == true && x.HanDung >= DateTime.Now); // online check
                var user = _context.Sys_Dm_User.FirstOrDefault(x => x.Username == checklogin.UserName && x.Password == PasswordEn);
                if (user == null)
                {
                    return new JsonResult(new { error = 1, ms = "Tài khoản hoặc mật khẩu không chính xác. Vui lòng kiểm tra lại!" });
                }
                if (useronline == 0)
                {
                    return new JsonResult(new { error = 1, ms = "Tài khoản hoặc mật khẩu không chính xác. Vui lòng kiểm tra lại!" });
                }
                int groupRoleDeFault = CheckPermission.getGroupRoleDefault(_context, user.Id);
                if (groupRoleDeFault == 0)
                {
                    return new JsonResult(new { error = 1, ms = "Tài khoản chưa được cấp quyền!. Vui lòng kiểm tra lại!" });
                }
                int perMission = CheckPermission.CheckPer(_context, user.Id, groupRoleDeFault);
                string tk = _authentication.GenerateToken("UserId", new RequestToken { UserID = user.Id });
                var congTys = await _context.Sys_Dm_Company.Where(x => x.IsActive == true).Select(a => new
                {
                    Name = "(" + a.Code + ") " + a.Name,
                    a.Id
                }).ToListAsync();

                switch (perMission)
                {
                    #region Nhóm quản trị tổng perMission = 0
                    case 0:
                        var companyId = congTys[0].Id;
                        var _listMenuAdmin = await (from b in _context.Sys_Dm_Menu.Where(x => x.IsActive == true)
                                                    select new
                                                    {
                                                        b.Id,
                                                        b.IsOrder,
                                                        name = b.Name,
                                                        url = b.RouterLink,
                                                        icon = b.IconMenu,
                                                        title = b.IsTitle,
                                                        b.MenuRank,
                                                        b.ParentId,
                                                    }).ToListAsync();
                        return new JsonResult(new
                        {
                            token = tk,
                            u = new
                            {
                                user.Id,
                                user.FullName,
                                user.CompanyId,
                                user.DepartmentId,
                                Permission = 0,
                                GroupRoleDeFault = groupRoleDeFault,
                                CompanyIdDefault = companyId,
                            }
                        ,
                            _listNhomQuyen = await (from a in _context.Sys_Cog_UsersGroup
                                                    join b in _context.Sys_Dm_GroupRole on a.GroupRoleId equals b.Id
                                                    where a.UserId == user.Id
                                                    orderby b.RankRole
                                                    select new
                                                    {
                                                        a.GroupRoleId,
                                                        b.Name
                                                    }).ToListAsync()
                        ,
                            _listQuyen = await (from b in _context.Sys_Dm_Menu.Where(x => x.IsActive == true)
                                                select new
                                                {
                                                    b.Id,
                                                    b.RouterLink,
                                                    ViewPer = true,
                                                    AddPer = true,
                                                    EditPer = true,
                                                    DelPer = true,
                                                    ExportPer = true,
                                                }).ToListAsync(),
                            data = _listMenuAdmin.Where(x => x.MenuRank < 3).Select(a => new
                            {
                                a.Id,
                                a.IsOrder,
                                a.name,
                                a.url,
                                a.icon,
                                a.MenuRank,
                                a.title,
                                children = _listMenuAdmin.Where(x => x.MenuRank >= 3 && x.ParentId == a.Id).Select(b => new
                                {
                                    b.Id,
                                    b.name,
                                    b.url,
                                    b.icon,
                                    b.title,
                                    b.IsOrder
                                }).OrderBy(y => y.IsOrder)
                            }).OrderBy(y => y.IsOrder),
                            congTys,
                            error = 0
                        });
                    #endregion
                    #region Nhóm quản trị công ty, chi nhánh
                    case 1:
                        var _listMenuCustomers = await (from a in _context.Sys_Cog_MenuCom
                                                        join b in _context.Sys_Dm_Menu on a.MenuId equals b.Id
                                                        where a.CompanyId == user.CompanyId && b.IsActive == true && a.IsActive == true
                                                        select new
                                                        {
                                                            name = b.Name,
                                                            url = b.RouterLink,
                                                            icon = b.IconMenu,
                                                            title = b.IsTitle,
                                                            b.ParentId,
                                                            b.MenuRank,
                                                            b.Id,
                                                            b.IsOrder,
                                                            ViewPer = true,
                                                            AddPer = true,
                                                            EditPer = true,
                                                            DelPer = true,
                                                            ExportPer = true,
                                                            b.RouterLink
                                                        }).ToListAsync();
                        var _listMenuExitst = await (from a in _context.Sys_Cog_MenuCom
                                                     where a.CompanyId == user.CompanyId && a.IsActive == true
                                                     group a by a.ParentId into c
                                                     select new
                                                     {
                                                         ParentId = c.Key
                                                     }).ToListAsync();
                        return new JsonResult(new
                        {
                            token = tk,
                            u = new
                            {
                                user.Id,
                                user.FullName,
                                user.CompanyId,
                                user.DepartmentId,
                                Permission = 1,
                                GroupRoleDeFault = groupRoleDeFault,
                                CompanyIdDefault = user.CompanyId,
                            }

                        ,
                            _listNhomQuyen = await (from a in _context.Sys_Cog_UsersGroup
                                                    join b in _context.Sys_Dm_GroupRole on a.GroupRoleId equals b.Id
                                                    where a.UserId == user.Id
                                                    orderby b.RankRole
                                                    select new
                                                    {
                                                        a.GroupRoleId,
                                                        b.Name
                                                    }).ToListAsync()
                        ,
                            _listQuyen = _listMenuCustomers.Select(a => new
                            {
                                a.Id,
                                a.AddPer,
                                a.ViewPer,
                                a.EditPer,
                                a.DelPer,
                                a.ExportPer,
                                a.RouterLink
                            }),
                            data = _listMenuCustomers.Where(x => x.MenuRank < 3 && _listMenuExitst.Count(e => e.ParentId == x.Id) > 0).Select(a => new
                            {
                                a.Id,
                                a.name,
                                a.url,
                                a.icon,
                                a.MenuRank,
                                a.title,
                                a.IsOrder,
                                children = _listMenuCustomers.Where(x => x.MenuRank >= 3 && x.ParentId == a.Id).Select(b => new
                                {
                                    b.Id,
                                    b.name,
                                    b.url,
                                    b.icon,
                                    b.title,
                                    b.IsOrder
                                }).OrderBy(y => y.IsOrder)
                            }).OrderBy(y => y.IsOrder),
                            congTys = new List<Sys_Dm_Company>(),
                            error = 0
                        });
                    #endregion
                    #region Nhóm quản trị phòng
                    case 2:
                        var _listMenuDepartments = await (from a in _context.Sys_Cog_MenuDep
                                                          join b in _context.Sys_Dm_Menu on a.MenuId equals b.Id
                                                          where a.DepartmentId == user.DepartmentId && b.IsActive == true && a.IsActive == true
                                                          select new
                                                          {
                                                              name = b.Name,
                                                              url = b.RouterLink,
                                                              icon = b.IconMenu,
                                                              title = b.IsTitle,
                                                              b.ParentId,
                                                              b.MenuRank,
                                                              b.Id,
                                                              b.IsOrder,
                                                              ViewPer = true,
                                                              AddPer = true,
                                                              EditPer = true,
                                                              DelPer = true,
                                                              ExportPer = true,
                                                              b.RouterLink
                                                          }).ToListAsync();
                        return new JsonResult(new
                        {
                            token = tk,
                            u = new
                            {
                                user.Id,
                                user.FullName,
                                user.DepartmentId,
                                user.CompanyId,
                                Permission = 2,
                                GroupRoleDeFault = groupRoleDeFault,
                                CompanyIdDefault = user.CompanyId
                            }
                            ,
                            _listNhomQuyen = await (from a in _context.Sys_Cog_UsersGroup
                                                    join b in _context.Sys_Dm_GroupRole on a.GroupRoleId equals b.Id
                                                    where a.UserId == user.Id
                                                    orderby b.RankRole
                                                    select new
                                                    {
                                                        a.GroupRoleId,
                                                        b.Name
                                                    }).ToListAsync()
                        ,
                            _listQuyen = _listMenuDepartments.Select(a => new
                            {
                                a.Id,
                                a.AddPer,
                                a.ViewPer,
                                a.EditPer,
                                a.DelPer,
                                a.ExportPer,
                                a.RouterLink
                            }),
                            data = _listMenuDepartments.Where(x => x.MenuRank < 3).Select(a => new
                            {
                                a.Id,
                                a.IsOrder,
                                a.name,
                                a.url,
                                a.icon,
                                a.MenuRank,
                                a.title,
                                children = _listMenuDepartments.Where(x => x.MenuRank >= 3 && x.ParentId == a.Id).Select(b => new
                                {
                                    b.Id,
                                    b.name,
                                    b.url,
                                    b.icon,
                                    b.title,
                                    b.IsOrder
                                }).OrderBy(y => y.IsOrder)
                            }).OrderBy(y => y.IsOrder),
                            congTys = new List<Sys_Dm_Company>(),
                            departments = new List<Sys_Dm_Department>(),
                            error = 0
                        });
                    #endregion
                    #region Nhóm quản trị tổ
                    case 3:
                        var _listMenuNest = await (from a in _context.Sys_Cog_MenuNest
                                                   join b in _context.Sys_Dm_Menu on a.MenuId equals b.Id
                                                   where a.DepartmentId == user.DepartmentId && b.IsActive == true && a.IsActive == true
                                                   select new
                                                   {
                                                       name = b.Name,
                                                       url = b.RouterLink,
                                                       icon = b.IconMenu,
                                                       title = b.IsTitle,
                                                       b.ParentId,
                                                       b.MenuRank,
                                                       b.Id,
                                                       b.IsOrder,
                                                       ViewPer = true,
                                                       AddPer = true,
                                                       EditPer = true,
                                                       DelPer = true,
                                                       ExportPer = true,
                                                       b.RouterLink
                                                   }).ToListAsync();
                        return new JsonResult(new
                        {
                            token = tk,
                            u = new
                            {
                                user.Id,
                                user.FullName,
                                user.DepartmentId,
                                user.CompanyId,
                                Permission = 3,
                                GroupRoleDeFault = groupRoleDeFault,
                                CompanyIdDefault = user.CompanyId
                            }
                            ,
                            _listNhomQuyen = await (from a in _context.Sys_Cog_UsersGroup
                                                    join b in _context.Sys_Dm_GroupRole on a.GroupRoleId equals b.Id
                                                    where a.UserId == user.Id
                                                    orderby b.RankRole
                                                    select new
                                                    {
                                                        a.GroupRoleId,
                                                        b.Name
                                                    }).ToListAsync()
                        ,
                            _listQuyen = _listMenuNest.Select(a => new
                            {
                                a.Id,
                                a.AddPer,
                                a.ViewPer,
                                a.EditPer,
                                a.DelPer,
                                a.ExportPer,
                                a.RouterLink
                            }),
                            data = _listMenuNest.Where(x => x.MenuRank < 3).Select(a => new
                            {
                                a.Id,
                                a.IsOrder,
                                a.name,
                                a.url,
                                a.icon,
                                a.MenuRank,
                                a.title,
                                children = _listMenuNest.Where(x => x.MenuRank >= 3 && x.ParentId == a.Id).Select(b => new
                                {
                                    b.Id,
                                    b.name,
                                    b.url,
                                    b.icon,
                                    b.title,
                                    b.IsOrder
                                }).OrderBy(y => y.IsOrder)
                            }).OrderBy(y => y.IsOrder),
                            congTys = new List<Sys_Dm_Company>(),
                            departments = new List<Sys_Dm_Department>(),
                            error = 0
                        });
                    #endregion
                    #region Nhóm thường
                    default:
                        var _listMenuNNormal = await (from a in _context.Sys_Cog_Permission
                                                      join b in _context.Sys_Dm_Menu on a.MenuId equals b.Id
                                                      where a.DepartmentId == user.DepartmentId && b.IsActive == true
                                                      where a.CompanyId == user.CompanyId && a.DepartmentId == user.DepartmentId && a.GroupRoleId == groupRoleDeFault && a.ViewPer == true
                                                      select new
                                                      {
                                                          name = b.Name,
                                                          url = b.RouterLink,
                                                          icon = b.IconMenu,
                                                          title = b.IsTitle,
                                                          b.ParentId,
                                                          b.MenuRank,
                                                          b.Id,
                                                          b.IsOrder,
                                                          ViewPer = true,
                                                          AddPer = true,
                                                          EditPer = true,
                                                          DelPer = true,
                                                          ExportPer = true,
                                                          b.RouterLink
                                                      }).ToListAsync();
                        return new JsonResult(new
                        {
                            token = tk,
                            u = new
                            {
                                user.Id,
                                user.FullName,
                                user.DepartmentId,
                                user.CompanyId,
                                Permission = 4,
                                GroupRoleDeFault = groupRoleDeFault,
                                CompanyIdDefault = user.CompanyId
                            }
                            ,
                            _listQuyen = _listMenuNNormal.Select(a => new
                            {
                                a.Id,
                                a.AddPer,
                                a.ViewPer,
                                a.EditPer,
                                a.DelPer,
                                a.ExportPer,
                                a.RouterLink
                            }),
                            data = _listMenuNNormal.Where(x => x.MenuRank < 3).Select(a => new
                            {
                                a.Id,
                                a.IsOrder,
                                a.name,
                                a.url,
                                a.icon,
                                a.MenuRank,
                                a.title,
                                children = _listMenuNNormal.Where(x => x.MenuRank >= 3 && x.ParentId == a.Id).Select(b => new
                                {
                                    b.Id,
                                    b.name,
                                    b.url,
                                    b.icon,
                                    b.title,
                                    b.IsOrder
                                }).OrderBy(y => y.IsOrder)
                            }).OrderBy(y => y.IsOrder),
                            congTys = new List<Sys_Dm_Company>(),
                            departments = new List<Sys_Dm_Department>(),
                            error = 0
                        });
                        #endregion
                }
            }
            catch (Exception)
            {
                return new ObjectResult(new {error = 1});
            }
           
           

        }
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<ActionResult<IEnumerable<Sys_Dm_User>>> ChangePassword(ChangePass change)
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var user = await _context.Sys_Dm_User.FindAsync(userId);
                string passwordOld = Helper.Encrypt(user.Username, change.PassOld);
                string passwordNew = Helper.Encrypt(user.Username, change.PassNew);
                if (passwordOld == user.Password)
                {
                    user.Password = passwordNew;
                    
                } else
                {
                    return new ObjectResult(new { error = 1 });
                }
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0 });
            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }
           
        }
    }
}