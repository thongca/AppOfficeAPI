using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HumanResource.Application.Helper.Dtos;
using HumanResource.Application.Paremeters;
using HumanResource.Application.Paremeters.Dtos;
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
    public class PermissionGroupController : ControllerBase
    {
        private readonly humanDbContext _context;
        public PermissionGroupController(humanDbContext context)
        {
            _context = context;
        }
        #region Module công ty
        // Post: api/PermissionGroup/r1GetListData
        [HttpPost]
        [Route("r1GetListData")]
        public async Task<ActionResult<IEnumerable<Sys_Cog_Permission>>> r1GetListData(Options options)
        {
            try
            {
                var tables = await (from a in _context.Sys_Cog_MenuCom
                                    join b in _context.Sys_Dm_Menu on a.MenuId equals b.Id
                                    where a.CompanyId == options.companyId
                                    orderby b.IsOrder
                                    select new
                                    {
                                        b.Id,
                                        b.Name,
                                        b.MenuRank,
                                        b.ParentId
                                    }).ToListAsync();
                var listMenuCom = tables.Where(x => x.MenuRank == 1).Select(a => new
                {
                    a.Name,
                    a.ParentId,
                    MenuId = a.Id,
                    a.MenuRank,
                    children = tables.Where(x => x.MenuRank == 2 && x.ParentId == a.Id).Select(b => new
                    {
                        b.Name,
                        b.ParentId,
                        MenuId = b.Id,
                        b.MenuRank,
                        children = tables.Where(x => x.MenuRank == 3 && x.ParentId == b.Id).Select(c => new
                        {
                            c.Name,
                            c.ParentId,
                            MenuId =c.Id,
                            c.MenuRank,
                            AddPer = (_context.Sys_Cog_Permission.FirstOrDefault(x => x.MenuId == c.Id && x.CompanyId == options.companyId && x.AddPer == true && x.GroupRoleId == options.groupId) != null) ? true : false,
                            ViewPer = (_context.Sys_Cog_Permission.FirstOrDefault(x => x.MenuId == c.Id && x.CompanyId == options.companyId && x.ViewPer == true && x.GroupRoleId == options.groupId) != null) ? true : false,
                            EditPer = (_context.Sys_Cog_Permission.FirstOrDefault(x => x.MenuId == c.Id && x.CompanyId == options.companyId && x.EditPer == true && x.GroupRoleId == options.groupId) != null) ? true : false,
                            DelPer = (_context.Sys_Cog_Permission.FirstOrDefault(x => x.MenuId == c.Id && x.CompanyId == options.companyId && x.DelPer == true && x.GroupRoleId == options.groupId) != null) ? true : false,
                            ExportPer = (_context.Sys_Cog_Permission.FirstOrDefault(x => x.MenuId == c.Id && x.CompanyId == options.companyId && x.ExportPer == true && x.GroupRoleId == options.groupId) != null) ? true : false
                        })
                    })
                });
                if (!string.IsNullOrEmpty(options.s))
                {
                    listMenuCom = listMenuCom.Where(c => c.Name.ToUpper().Contains(options.s.ToUpper()));
                }
                return new ObjectResult(new { error = 0, data = listMenuCom, total = listMenuCom.Count() });
            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }
        }
        // Post: api/PermissionGroup/r2AddDataModel
        [HttpPost]
        [Route("r2AddDataModel")]
        public async Task<ActionResult<IEnumerable<Sys_Cog_Permission>>> r2AddDataModel(MenuPermissionCom options)
        {
            try
            {
                if (options.NestId > 0)
                {
                    options.DepartmentId = options.NestId;
                }
                 RequestToken token = CommonData.GetDataFromToken(User);
                var menuThree = await _context.Sys_Dm_Menu.FindAsync(options.MenuId);
                var menuComThree = _context.Sys_Cog_Permission.Count(x => x.MenuId == menuThree.Id && x.CompanyId == options.CompanyId && x.GroupRoleId == options.GroupRoleId);
                if (menuComThree == 0)
                {
                    var menuTwo = await _context.Sys_Dm_Menu.FindAsync(menuThree.ParentId);
                    var menuComTwo = _context.Sys_Cog_Permission.Count(x => x.MenuId == menuThree.ParentId && x.CompanyId == options.CompanyId && x.GroupRoleId == options.GroupRoleId);
                    if (menuComTwo == 0)
                    {
                        var menuOne = await _context.Sys_Dm_Menu.FindAsync(menuTwo.ParentId);
                        var menuComOne = _context.Sys_Cog_Permission.Count(x => x.MenuId == menuTwo.ParentId && x.CompanyId == options.CompanyId && x.GroupRoleId == options.GroupRoleId);
                        if (menuComOne == 0)
                        {
                            Sys_Cog_Permission objOne = new Sys_Cog_Permission();
                            objOne.MenuId = menuOne.Id;
                            objOne.GroupRoleId = options.GroupRoleId;
                            objOne.DepartmentId = options.DepartmentId;
                            objOne.CompanyId = options.CompanyId;
                            objOne.IsActive = true;
                            objOne.AddPer = options.AddPer ?? false;
                            objOne.ViewPer = options.ViewPer ?? false;
                            objOne.EditPer = options.EditPer ?? false;
                            objOne.ExportPer = options.ExportPer ?? false;
                            objOne.DelPer = options.DelPer ?? false;
                            objOne.UserCreateId = token.UserID;
                            objOne.CreatDate = DateTime.Now;
                            objOne.ParentId = null;
                            _context.Sys_Cog_Permission.Add(objOne);
                            Sys_Cog_Permission objTwo = new Sys_Cog_Permission();
                            objTwo.MenuId = menuTwo.Id;
                            objTwo.GroupRoleId = options.GroupRoleId;
                            objTwo.CompanyId = options.CompanyId;
                            objTwo.DepartmentId = options.DepartmentId;
                            objTwo.IsActive = true;
                            objTwo.AddPer = options.AddPer ?? false;
                            objTwo.ViewPer = options.ViewPer ?? false;
                            objTwo.EditPer = options.EditPer ?? false;
                            objTwo.ExportPer = options.ExportPer ?? false;
                            objTwo.DelPer = options.DelPer ?? false;
                            objTwo.UserCreateId = token.UserID;
                            objTwo.CreatDate = DateTime.Now;
                            objTwo.ParentId = menuOne.Id;
                            _context.Sys_Cog_Permission.Add(objTwo);
                            Sys_Cog_Permission objThree = new Sys_Cog_Permission();
                            objThree.MenuId = options.MenuId;
                            objThree.GroupRoleId = options.GroupRoleId;
                            objThree.DepartmentId = options.DepartmentId;
                            objThree.CompanyId = options.CompanyId;
                            objThree.IsActive = true;
                            objThree.AddPer = options.AddPer ?? false;
                            objThree.ViewPer = options.ViewPer ?? false;
                            objThree.EditPer = options.EditPer ?? false;
                            objThree.ExportPer = options.ExportPer ?? false;
                            objThree.DelPer = options.DelPer ?? false;
                            objThree.UserCreateId = token.UserID;
                            objThree.CreatDate = DateTime.Now;
                            objThree.ParentId = menuTwo.Id;
                            _context.Sys_Cog_Permission.Add(objThree);
                        }
                        else
                        {
                            Sys_Cog_Permission objTwo = new Sys_Cog_Permission();
                            objTwo.MenuId = menuTwo.Id;
                            objTwo.GroupRoleId = options.GroupRoleId;
                            objTwo.DepartmentId = options.DepartmentId;
                            objTwo.CompanyId = options.CompanyId;
                            objTwo.IsActive = true;
                            objTwo.AddPer = options.AddPer ?? false;
                            objTwo.ViewPer = options.ViewPer ?? false;
                            objTwo.EditPer = options.EditPer ?? false;
                            objTwo.ExportPer = options.ExportPer ?? false;
                            objTwo.DelPer = options.DelPer ?? false;
                            objTwo.UserCreateId = token.UserID;
                            objTwo.CreatDate = DateTime.Now;
                            objTwo.ParentId = menuOne.Id;
                            _context.Sys_Cog_Permission.Add(objTwo);
                            Sys_Cog_Permission objThree = new Sys_Cog_Permission();
                            objThree.MenuId = options.MenuId;
                            objThree.GroupRoleId = options.GroupRoleId;
                            objThree.DepartmentId = options.DepartmentId;
                            objThree.CompanyId = options.CompanyId;
                            objThree.IsActive = true;
                            objThree.AddPer = options.AddPer ?? false;
                            objThree.ViewPer = options.ViewPer ?? false;
                            objThree.EditPer = options.EditPer ?? false;
                            objThree.ExportPer = options.ExportPer ?? false;
                            objThree.DelPer = options.DelPer ?? false;
                            objThree.UserCreateId = token.UserID;
                            objThree.CreatDate = DateTime.Now;
                            objThree.ParentId = menuTwo.Id;
                            _context.Sys_Cog_Permission.Add(objThree);
                        }
                    }
                    else
                    {
                        var menuComTwoParent = await _context.Sys_Cog_Permission.FirstOrDefaultAsync(x => x.MenuId == menuThree.ParentId && x.CompanyId == options.CompanyId && x.GroupRoleId == options.GroupRoleId);
                        Sys_Cog_Permission objThree = new Sys_Cog_Permission();
                        objThree.MenuId = options.MenuId;
                        objThree.GroupRoleId = options.GroupRoleId;
                        objThree.CompanyId = options.CompanyId;
                        objThree.DepartmentId = options.DepartmentId;
                        objThree.IsActive = true;
                        objThree.AddPer = options.AddPer ?? false;
                        objThree.ViewPer = options.ViewPer ?? false;
                        objThree.EditPer = options.EditPer ?? false;
                        objThree.ExportPer = options.ExportPer ?? false;
                        objThree.DelPer = options.DelPer ?? false;
                        objThree.UserCreateId = token.UserID;
                        objThree.CreatDate = DateTime.Now;
                        objThree.ParentId = menuTwo.Id;
                        _context.Sys_Cog_Permission.Add(objThree);
                      
                        #region ViewPer three nhap vao = true

                            if (_context.Sys_Cog_Permission.Count(x => x.ViewPer == true && x.ParentId == menuThree.ParentId && x.GroupRoleId == options.GroupRoleId && x.CompanyId == options.CompanyId && x.MenuId != options.MenuId) == 0)
                            {
                                var menuTwoParent = await _context.Sys_Cog_Permission.FirstOrDefaultAsync(x => x.MenuId == menuThree.ParentId && x.CompanyId == options.CompanyId && x.GroupRoleId == options.GroupRoleId);
                                menuTwoParent.ViewPer = true;
                                if (_context.Sys_Cog_Permission.Count(x => x.ViewPer == true && x.ParentId == menuTwoParent.ParentId && x.GroupRoleId == options.GroupRoleId && x.CompanyId == options.CompanyId && x.MenuId != menuTwoParent.MenuId) == 0)
                                {
                                    var menuoneParent = await _context.Sys_Cog_Permission.FirstOrDefaultAsync(x => x.MenuId == menuTwoParent.ParentId && x.CompanyId == options.CompanyId && x.GroupRoleId == options.GroupRoleId);
                                    menuoneParent.ViewPer = true;
                                }
                            }
                        
                        #endregion
                    }

                }
                else
                {

                    var menuCome = await _context.Sys_Cog_Permission.FirstOrDefaultAsync(x => x.MenuId == options.MenuId && x.CompanyId == options.CompanyId && x.GroupRoleId == options.GroupRoleId);
                    #region Viewper nhap vao = false
                    if (options.ViewPer == false && menuCome.ViewPer == true)
                    {
                        if (_context.Sys_Cog_Permission.Count(x => x.ViewPer == true && x.ParentId == menuCome.ParentId && x.GroupRoleId == options.GroupRoleId && x.CompanyId == options.CompanyId && x.MenuId != options.MenuId) == 0)
                        {
                            var menuTwoParent = await _context.Sys_Cog_Permission.FirstOrDefaultAsync(x => x.MenuId == menuCome.ParentId && x.CompanyId == options.CompanyId && x.GroupRoleId == options.GroupRoleId);
                            menuTwoParent.ViewPer = false;
                            if (_context.Sys_Cog_Permission.Count(x => x.ViewPer == true && x.ParentId == menuTwoParent.ParentId && x.GroupRoleId == options.GroupRoleId && x.CompanyId == options.CompanyId && x.MenuId != menuTwoParent.MenuId) == 0)
                            {
                                var menuoneParent = await _context.Sys_Cog_Permission.FirstOrDefaultAsync(x => x.MenuId == menuTwoParent.ParentId && x.CompanyId == options.CompanyId && x.GroupRoleId == options.GroupRoleId);
                                menuoneParent.ViewPer = false;
                            }
                        }
                    }
                    #endregion
                    #region ViewPer three nhap vao = true
                    if (options.ViewPer == true && menuCome.ViewPer == false)
                    {
                        if (_context.Sys_Cog_Permission.Count(x => x.ViewPer == true && x.ParentId == menuCome.ParentId && x.GroupRoleId == options.GroupRoleId && x.CompanyId == options.CompanyId && x.MenuId != options.MenuId) == 0)
                        {
                            var menuTwoParent = await _context.Sys_Cog_Permission.FirstOrDefaultAsync(x => x.MenuId == menuCome.ParentId && x.CompanyId == options.CompanyId && x.GroupRoleId == options.GroupRoleId);
                            menuTwoParent.ViewPer = true;
                            if (_context.Sys_Cog_Permission.Count(x => x.ViewPer == true && x.ParentId == menuTwoParent.ParentId && x.GroupRoleId == options.GroupRoleId && x.CompanyId == options.CompanyId && x.MenuId != menuTwoParent.MenuId) == 0)
                            {
                                var menuoneParent = await _context.Sys_Cog_Permission.FirstOrDefaultAsync(x => x.MenuId == menuTwoParent.ParentId && x.CompanyId == options.CompanyId && x.GroupRoleId == options.GroupRoleId);
                                menuoneParent.ViewPer = true;
                            }
                        }
                    }
                    #endregion
                    menuCome.AddPer = options.AddPer ?? false;
                    menuCome.ViewPer = options.ViewPer ?? false;
                    menuCome.EditPer = options.EditPer ?? false;
                    menuCome.ExportPer = options.ExportPer ?? false;
                    menuCome.DelPer = options.DelPer ?? false;
                   
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
        #region module theo phòng ban
        // Post: api/PermissionGroup/r1GetListDataDepartment
        [HttpPost]
        [Route("r1GetListDataDepartment")]
        public async Task<ActionResult<IEnumerable<Sys_Cog_MenuDep>>> r1GetListDataDepartment(Options options)
        {
            try
            {
                var tables = await (from a in _context.Sys_Cog_MenuDep
                                    join b in _context.Sys_Dm_Menu on a.MenuId equals b.Id
                                    where a.CompanyId == options.companyId && a.IsActive == true && a.DepartmentId == options.departmentId
                                    orderby b.IsOrder
                                    select new
                                    {
                                        MenuId = b.Id,
                                        b.Name,
                                        b.MenuRank,
                                        a.ParentId,
                                        a.IsActive
                                    }).ToListAsync();
                var exits = from a in _context.Sys_Cog_MenuCom
                            where a.CompanyId == options.companyId && a.IsActive == true
                            group a by a.ParentId into c
                            select new
                            {
                                ParentId = c.Key,
                            };
                var listMenuCDep = tables.Where(x => x.MenuRank == 1 && exits.Count(e => e.ParentId == x.MenuId) > 0).Select(a => new
                {
                    a.Name,
                    a.ParentId,
                    a.MenuId,
                    a.MenuRank,
                    children = tables.Where(x => x.MenuRank == 2 && x.ParentId == a.MenuId && x.IsActive == true && exits.Count(e => e.ParentId == x.MenuId) > 0).Select(b => new
                    {
                        b.Name,
                        b.ParentId,
                        b.MenuId,
                        b.MenuRank,
                        children = tables.Where(x => x.MenuRank == 3 && x.ParentId == b.MenuId && x.IsActive == true).Select(c => new
                        {
                            c.Name,
                            c.ParentId,
                            c.MenuId,
                            c.MenuRank,
                            AddPer = (_context.Sys_Cog_Permission.FirstOrDefault(x => x.MenuId == c.MenuId && x.CompanyId == options.companyId && x.AddPer == true && x.GroupRoleId == options.groupId && x.DepartmentId == options.departmentId) != null) ? true : false,
                            ViewPer = (_context.Sys_Cog_Permission.FirstOrDefault(x => x.MenuId == c.MenuId && x.CompanyId == options.companyId && x.ViewPer == true && x.GroupRoleId == options.groupId && x.DepartmentId == options.departmentId) != null) ? true : false,
                            EditPer = (_context.Sys_Cog_Permission.FirstOrDefault(x => x.MenuId == c.MenuId && x.CompanyId == options.companyId && x.EditPer == true && x.GroupRoleId == options.groupId && x.DepartmentId == options.departmentId) != null) ? true : false,
                            DelPer = (_context.Sys_Cog_Permission.FirstOrDefault(x => x.MenuId == c.MenuId && x.CompanyId == options.companyId && x.DelPer == true && x.GroupRoleId == options.groupId && x.DepartmentId == options.departmentId) != null) ? true : false,
                            ExportPer = (_context.Sys_Cog_Permission.FirstOrDefault(x => x.MenuId == c.MenuId && x.CompanyId == options.companyId && x.ExportPer == true && x.GroupRoleId == options.groupId && x.DepartmentId == options.departmentId) != null) ? true : false
                        })
                    })
                });
                if (!string.IsNullOrEmpty(options.s))
                {
                    listMenuCDep = listMenuCDep.Where(c => c.Name.ToUpper().Contains(options.s.ToUpper()));
                }
                return new ObjectResult(new { error = 0, data = listMenuCDep, total = listMenuCDep.Count() });
            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region module theo tổ
        // Post: api/PermissionGroup/r1GetListDataNest
        [HttpPost]
        [Route("r1GetListDataNest")]
        public async Task<ActionResult<IEnumerable<Sys_Cog_MenuDep>>> r1GetListDataNest(Options options)
        {
            try
            {
                var tables = await (from a in _context.Sys_Cog_MenuNest
                                    join b in _context.Sys_Dm_Menu on a.MenuId equals b.Id
                                    where a.CompanyId == options.companyId && a.IsActive == true && a.DepartmentId == options.nestId
                                    orderby b.IsOrder
                                    select new
                                    {
                                        MenuId = b.Id,
                                        b.Name,
                                        b.MenuRank,
                                        a.ParentId,
                                        a.IsActive
                                    }).ToListAsync();
                var exits = from a in _context.Sys_Cog_MenuCom
                            where a.CompanyId == options.companyId && a.IsActive == true
                            group a by a.ParentId into c
                            select new
                            {
                                ParentId = c.Key,
                            };
                var listMenuCDep = tables.Where(x => x.MenuRank == 1 && exits.Count(e => e.ParentId == x.MenuId) > 0).Select(a => new
                {
                    a.Name,
                    a.ParentId,
                    a.MenuId,
                    a.MenuRank,
                    children = tables.Where(x => x.MenuRank == 2 && x.ParentId == a.MenuId && x.IsActive == true && exits.Count(e => e.ParentId == x.MenuId) > 0).Select(b => new
                    {
                        b.Name,
                        b.ParentId,
                        b.MenuId,
                        b.MenuRank,
                        children = tables.Where(x => x.MenuRank == 3 && x.ParentId == b.MenuId && x.IsActive == true).Select(c => new
                        {
                            c.Name,
                            c.ParentId,
                            c.MenuId,
                            c.MenuRank,
                            AddPer = (_context.Sys_Cog_Permission.FirstOrDefault(x => x.MenuId == c.MenuId && x.CompanyId == options.companyId && x.AddPer == true && x.GroupRoleId == options.groupId && x.DepartmentId == options.nestId) != null) ? true : false,
                            ViewPer = (_context.Sys_Cog_Permission.FirstOrDefault(x => x.MenuId == c.MenuId && x.CompanyId == options.companyId && x.ViewPer == true && x.GroupRoleId == options.groupId && x.DepartmentId == options.nestId) != null) ? true : false,
                            EditPer = (_context.Sys_Cog_Permission.FirstOrDefault(x => x.MenuId == c.MenuId && x.CompanyId == options.companyId && x.EditPer == true && x.GroupRoleId == options.groupId && x.DepartmentId == options.nestId) != null) ? true : false,
                            DelPer = (_context.Sys_Cog_Permission.FirstOrDefault(x => x.MenuId == c.MenuId && x.CompanyId == options.companyId && x.DelPer == true && x.GroupRoleId == options.groupId && x.DepartmentId == options.nestId) != null) ? true : false,
                            ExportPer = (_context.Sys_Cog_Permission.FirstOrDefault(x => x.MenuId == c.MenuId && x.CompanyId == options.companyId && x.ExportPer == true && x.GroupRoleId == options.groupId && x.DepartmentId == options.nestId) != null) ? true : false
                        })
                    })
                });
                if (!string.IsNullOrEmpty(options.s))
                {
                    listMenuCDep = listMenuCDep.Where(c => c.Name.ToUpper().Contains(options.s.ToUpper()));
                }
                return new ObjectResult(new { error = 0, data = listMenuCDep, total = listMenuCDep.Count() });
            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
    }
}