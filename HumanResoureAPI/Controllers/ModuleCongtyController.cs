﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using HumanResource.Application.Paremeters;
using HumanResource.Application.Paremeters.Dtos;
using HumanResource.Data.EF;
using HumanResource.Data.Entities.System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HumanResoureAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModuleCongtyController : ControllerBase
    {
        private readonly humanDbContext _context;
        public ModuleCongtyController(humanDbContext context)
        {
            _context = context;
        }
        #region module theo company
        // Post: api/ModuleCongty/r1GetListData
        [HttpPost]
        [Route("r1GetListData")]
        public async Task<ActionResult<IEnumerable<Sys_Cog_MenuCom>>> r1GetListData(Options options)
        {
            try
            {
                var tables = await (from a in _context.Sys_Dm_Menu
                                    orderby a.IsOrder
                                    select new
                                    {
                                        a.Id,
                                        a.Name,
                                        a.MenuRank,
                                        a.ParentId
                                    }).ToListAsync();
                var listMenuCom = tables.Where(x => x.MenuRank == 1).Select(a => new
                {
                    a.Name,
                    a.ParentId,
                    a.Id,
                    a.MenuRank,
                    children = tables.Where(x => x.MenuRank == 2 && x.ParentId == a.Id).Select(b => new
                    {
                        b.Name,
                        b.ParentId,
                        b.Id,
                        b.MenuRank,
                        children = tables.Where(x => x.MenuRank == 3 && x.ParentId == b.Id).Select(c => new
                        {
                            c.Name,
                            c.ParentId,
                            c.Id,
                            c.MenuRank,
                            IsActive = (_context.Sys_Cog_MenuCom.FirstOrDefault(x => x.MenuId == c.Id && x.CompanyId == options.companyId && x.IsActive == true) != null) ? true : false
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
        [HttpPost]
        [Route("r2AddDataModel")]
        public async Task<ActionResult<IEnumerable<Sys_Cog_MenuCom>>> r2AddDataModel(MenuCongtyOp options)
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var menuThree = await _context.Sys_Dm_Menu.FindAsync(options.Id);
                var menuComThree =  _context.Sys_Cog_MenuCom.Count(x=>x.MenuId == menuThree.Id && x.CompanyId == options.CompanyId);
                if (menuComThree == 0)
                {
                    var menuTwo = await _context.Sys_Dm_Menu.FindAsync(menuThree.ParentId);
                    var menuComTwo = _context.Sys_Cog_MenuCom.Count(x => x.MenuId == menuThree.ParentId && x.CompanyId == options.CompanyId);
                    if (menuComTwo == 0)
                    {
                        var menuOne = await _context.Sys_Dm_Menu.FindAsync(menuTwo.ParentId);
                        var menuComOne = _context.Sys_Cog_MenuCom.Count(x => x.MenuId == menuTwo.ParentId && x.CompanyId == options.CompanyId);
                        if (menuComOne == 0)
                        {
                            Sys_Cog_MenuCom objOne = new Sys_Cog_MenuCom();
                            objOne.MenuId = menuOne.Id;
                            objOne.CompanyId = options.CompanyId;
                            objOne.IsActive = true;
                            objOne.ParentId = null;
                            objOne.UserUpdateId = userId;
                            objOne.DateUpdate = DateTime.Now;
                            _context.Sys_Cog_MenuCom.Add(objOne);
                            Sys_Cog_MenuCom objTwo = new Sys_Cog_MenuCom();
                            objTwo.MenuId = menuTwo.Id;
                            objTwo.CompanyId = options.CompanyId;
                            objTwo.IsActive = true;
                            objTwo.ParentId = menuOne.Id;
                            objTwo.UserUpdateId = userId;
                            objTwo.DateUpdate = DateTime.Now;
                            _context.Sys_Cog_MenuCom.Add(objTwo);
                            Sys_Cog_MenuCom objThree = new Sys_Cog_MenuCom();
                            objThree.MenuId = options.Id;
                            objThree.CompanyId = options.CompanyId;
                            objThree.IsActive = true;
                            objThree.ParentId = menuTwo.Id;
                            objThree.UserUpdateId = userId;
                            objThree.DateUpdate = DateTime.Now;
                            _context.Sys_Cog_MenuCom.Add(objThree);
                        }
                        else
                        {
                            Sys_Cog_MenuCom objTwo = new Sys_Cog_MenuCom();
                            objTwo.MenuId = menuTwo.Id;
                            objTwo.CompanyId = options.CompanyId;
                            objTwo.IsActive = true;
                            objTwo.ParentId = menuOne.Id;
                            objTwo.UserUpdateId = userId;
                            objTwo.DateUpdate = DateTime.Now;
                            _context.Sys_Cog_MenuCom.Add(objTwo);
                            Sys_Cog_MenuCom objThree = new Sys_Cog_MenuCom();
                            objThree.MenuId = options.Id;
                            objThree.CompanyId = options.CompanyId;
                            objThree.ParentId = menuTwo.Id;
                            objThree.IsActive = true;
                            objThree.UserUpdateId = userId;
                            objThree.DateUpdate = DateTime.Now;
                            _context.Sys_Cog_MenuCom.Add(objThree);
                        }
                    }
                    else
                    {
                        var menuComTwoParent = await _context.Sys_Cog_MenuCom.FirstOrDefaultAsync(x => x.MenuId == menuThree.ParentId && x.CompanyId == options.CompanyId);
                        Sys_Cog_MenuCom objThree = new Sys_Cog_MenuCom();
                        objThree.MenuId = options.Id;
                        objThree.CompanyId = options.CompanyId;
                        objThree.IsActive = true;
                        objThree.ParentId = menuTwo.Id;
                        objThree.UserUpdateId = userId;
                        objThree.DateUpdate = DateTime.Now;
                        _context.Sys_Cog_MenuCom.Add(objThree);
                        menuComTwoParent.IsActive = true;
                    }

                }
                else
                {
                    var menuCome =await _context.Sys_Cog_MenuCom.FirstOrDefaultAsync(x=>x.MenuId == options.Id && x.CompanyId == options.CompanyId);
                    var menuComTwoParent =await _context.Sys_Cog_MenuCom.FirstOrDefaultAsync(x=>x.MenuId == menuThree.ParentId && x.CompanyId == options.CompanyId);
                    if (options.IsActive == false)
                    {
                        var rmmenuDeps = _context.Sys_Cog_MenuDep.Where(x => x.MenuId == options.Id && x.CompanyId == options.CompanyId).ToList(); // xóa menu phòng
                       
                        if (_context.Sys_Cog_MenuCom.Count(x=>x.ParentId == menuThree.ParentId && x.CompanyId == options.CompanyId && x.IsActive == true && x.MenuId != options.Id) == 0)
                        {
                            menuComTwoParent.IsActive = false;
                        }
                        foreach (var item in rmmenuDeps)
                        {
                            item.IsActive = false;
                            var menuDepParent = await _context.Sys_Cog_MenuDep.FirstOrDefaultAsync(x => x.MenuId == item.ParentId && x.CompanyId == item.CompanyId && x.DepartmentId == item.DepartmentId);
                            if (_context.Sys_Cog_MenuDep.Count(x => x.ParentId == item.ParentId && x.CompanyId == item.CompanyId && x.IsActive == true && x.MenuId != item.MenuId) == 0)
                            {
                                menuDepParent.IsActive = false;
                            }
                            var rmmenuNests = _context.Sys_Cog_MenuNest.Where(x => x.MenuId == options.Id && x.CompanyId == options.CompanyId && x.ParentDepartmentId == item.DepartmentId).ToList(); // xóa menu phòng
                            foreach (var ntem in rmmenuNests)
                            {
                                ntem.IsActive = false;
                            }
                        }
                        menuCome.IsActive = false;
                        menuCome.DateUpdate = DateTime.Now;
                        menuCome.UserUpdateId = userId;
                    }
                    else
                    {
                        if (_context.Sys_Cog_MenuCom.Count(x => x.ParentId == menuThree.ParentId && x.CompanyId == options.CompanyId && x.IsActive == true) == 0)
                        {
                            menuComTwoParent.IsActive = true;
                        }
                        menuCome.IsActive = true;
                        menuCome.DateUpdate = DateTime.Now;
                        menuCome.UserUpdateId = userId;
                    }
                }
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0 });
            }
            catch (Exception ez)
            {
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region module theo phòng ban
        // Post: api/ModuleCongty/r1GetListData
        [HttpPost]
        [Route("r1GetListDataDepartment")]
        public async Task<ActionResult<IEnumerable<Sys_Cog_MenuDep>>> r1GetListDataDepartment(Options options)
        {
            try
            {
                var tables = await (from a in _context.Sys_Cog_MenuCom
                                    join b in _context.Sys_Dm_Menu on a.MenuId equals b.Id
                                    where a.CompanyId == options.companyId && a.IsActive == true
                                    orderby b.IsOrder
                                    select new
                                    {
                                        b.Id,
                                        b.Name,
                                        b.MenuRank,
                                        a.ParentId,
                                        a.IsActive
                                    }).ToListAsync();
                var exits =  from a in _context.Sys_Cog_MenuCom
                                    where a.CompanyId == options.companyId && a.IsActive == true 
                                    group a by a.ParentId into c
                                    select new
                                    {
                                        ParentId = c.Key,
                                    };
                var listMenuCDep = tables.Where(x => x.MenuRank == 1 && exits.Count(e=>e.ParentId == x.Id) > 0).Select(a => new
                {
                    a.Name,
                    a.ParentId,
                    a.Id,
                    a.MenuRank,
                    children = tables.Where(x => x.MenuRank == 2 && x.ParentId == a.Id && x.IsActive == true && exits.Count(e => e.ParentId == x.Id) > 0).Select(b => new
                    {
                        b.Name,
                        b.ParentId,
                        b.Id,
                        b.MenuRank,
                        children = tables.Where(x => x.MenuRank == 3 && x.ParentId == b.Id && x.IsActive == true).Select(c => new
                        {
                            c.Name,
                            c.ParentId,
                            c.Id,
                            c.MenuRank,
                            IsActive = (_context.Sys_Cog_MenuDep.FirstOrDefault(x => x.MenuId == c.Id && x.CompanyId == options.companyId && x.DepartmentId == options.departmentId && x.IsActive == true) != null) ? true : false
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
        [HttpPost]
        [Route("r2AddDataModelDepartment")]
        public async Task<ActionResult<IEnumerable<Sys_Cog_MenuDep>>> r2AddDataModelDepartment(MenuCongtyOp options)
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var menuThree = await _context.Sys_Dm_Menu.FindAsync(options.Id);
                var menuComThree = _context.Sys_Cog_MenuDep.Count(x => x.MenuId == menuThree.Id && x.CompanyId == options.CompanyId && x.DepartmentId == options.DepartmentId);
                if (menuComThree == 0)
                {
                    var menuTwo = await _context.Sys_Dm_Menu.FindAsync(menuThree.ParentId);
                    var menuComTwo = _context.Sys_Cog_MenuDep.Count(x => x.MenuId == menuThree.ParentId && x.CompanyId == options.CompanyId && x.DepartmentId == options.DepartmentId);
                    if (menuComTwo == 0)
                    {
                        var menuOne = await _context.Sys_Dm_Menu.FindAsync(menuTwo.ParentId);
                        var menuComOne = _context.Sys_Cog_MenuDep.Count(x => x.MenuId == menuTwo.ParentId && x.CompanyId == options.CompanyId && x.DepartmentId == options.DepartmentId);
                        if (menuComOne == 0)
                        {
                            Sys_Cog_MenuDep objOne = new Sys_Cog_MenuDep();
                            objOne.MenuId = menuOne.Id;
                            objOne.CompanyId = options.CompanyId;
                            objOne.IsActive = true;
                            objOne.ParentId = null;
                            objOne.DepartmentId = options.DepartmentId;
                            objOne.UserUpdateId = userId;
                            objOne.DateUpdate = DateTime.Now;
                            _context.Sys_Cog_MenuDep.Add(objOne);
                            Sys_Cog_MenuDep objTwo = new Sys_Cog_MenuDep();
                            objTwo.MenuId = menuTwo.Id;
                            objTwo.CompanyId = options.CompanyId;
                            objTwo.IsActive = true;
                            objTwo.ParentId = menuOne.Id;
                            objTwo.DepartmentId = options.DepartmentId;
                            objTwo.UserUpdateId = userId;
                            objTwo.DateUpdate = DateTime.Now;
                            _context.Sys_Cog_MenuDep.Add(objTwo);
                            Sys_Cog_MenuDep objThree = new Sys_Cog_MenuDep();
                            objThree.MenuId = options.Id;
                            objThree.CompanyId = options.CompanyId;
                            objThree.IsActive = true;
                            objThree.ParentId = menuTwo.Id;
                            objThree.DepartmentId = options.DepartmentId;
                            objThree.UserUpdateId = userId;
                            objThree.DateUpdate = DateTime.Now;
                            _context.Sys_Cog_MenuDep.Add(objThree);
                        }
                        else
                        {
                            Sys_Cog_MenuDep objTwo = new Sys_Cog_MenuDep();
                            objTwo.MenuId = menuTwo.Id;
                            objTwo.CompanyId = options.CompanyId;
                            objTwo.IsActive = true;
                            objTwo.ParentId = menuOne.Id;
                            objTwo.DepartmentId = options.DepartmentId;
                            objTwo.UserUpdateId = userId;
                            objTwo.DateUpdate = DateTime.Now;
                            _context.Sys_Cog_MenuDep.Add(objTwo);
                            Sys_Cog_MenuDep objThree = new Sys_Cog_MenuDep();
                            objThree.MenuId = options.Id;
                            objThree.CompanyId = options.CompanyId;
                            objThree.ParentId = menuTwo.Id;
                            objThree.IsActive = true;
                            objThree.DepartmentId = options.DepartmentId;
                            objThree.UserUpdateId = userId;
                            objThree.DateUpdate = DateTime.Now;
                            _context.Sys_Cog_MenuDep.Add(objThree);
                        }
                    }
                    else
                    {
                        var menuComTwoParent = await _context.Sys_Cog_MenuDep.FirstOrDefaultAsync(x => x.MenuId == menuThree.ParentId && x.CompanyId == options.CompanyId && x.DepartmentId == options.DepartmentId);
                        Sys_Cog_MenuDep objThree = new Sys_Cog_MenuDep();
                        objThree.MenuId = options.Id;
                        objThree.CompanyId = options.CompanyId;
                        objThree.IsActive = true;
                        objThree.ParentId = menuTwo.Id;
                        objThree.DepartmentId = options.DepartmentId;
                        objThree.UserUpdateId = userId;
                        objThree.DateUpdate = DateTime.Now;
                        _context.Sys_Cog_MenuDep.Add(objThree);
                        menuComTwoParent.IsActive = true;
                    }

                }
                else
                {
                    var menuCome = await _context.Sys_Cog_MenuDep.FirstOrDefaultAsync(x => x.MenuId == options.Id && x.CompanyId == options.CompanyId && x.DepartmentId == options.DepartmentId);
                    var menuComTwoParent = await _context.Sys_Cog_MenuDep.FirstOrDefaultAsync(x => x.MenuId == menuThree.ParentId && x.CompanyId == options.CompanyId && x.DepartmentId == options.DepartmentId);
                    
                    if (options.IsActive == false)
                    {
                        var rmmenuNest = _context.Sys_Cog_MenuNest.Where(x => x.MenuId == options.Id && x.CompanyId == options.CompanyId && x.ParentDepartmentId == options.DepartmentId); // xóa menu tổ
                        if (_context.Sys_Cog_MenuDep.Count(x => x.ParentId == menuThree.ParentId && x.CompanyId == options.CompanyId && x.IsActive == true && x.MenuId != options.Id && x.DepartmentId == options.DepartmentId) == 0)
                        {
                            menuComTwoParent.IsActive = false;
                        }
                        menuCome.IsActive = false;
                        foreach (var item in rmmenuNest)
                        {
                            item.IsActive = false;

                        }
                        menuCome.UserUpdateId = userId;
                        menuCome.DateUpdate = DateTime.Now;
                    }
                    else
                    {
                        if (menuComTwoParent.IsActive == false)
                        {
                            menuComTwoParent.IsActive = true;
                        }
                        menuCome.IsActive = true;
                        menuCome.UserUpdateId = userId;
                        menuCome.DateUpdate = DateTime.Now;
                    }
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
        #region module theo tổ
        // Post: api/ModuleCongty/r1GetListData
        [HttpPost]
        [Route("r1GetListDataNest")]
        public async Task<ActionResult<IEnumerable<Sys_Cog_MenuDep>>> r1GetListDataNest(Options options)
        {
            try
            {
                var tables = await (from a in _context.Sys_Cog_MenuDep
                                    join b in _context.Sys_Dm_Menu on a.MenuId equals b.Id
                                    where a.CompanyId == options.companyId && a.IsActive == true && a.DepartmentId == options.departmentId
                                    orderby b.IsOrder
                                    select new
                                    {
                                        b.Id,
                                        b.Name,
                                        b.MenuRank,
                                        a.ParentId,
                                        a.IsActive
                                    }).ToListAsync();
                var exits = from a in _context.Sys_Cog_MenuDep
                            where a.CompanyId == options.companyId && a.IsActive == true && a.DepartmentId == options.departmentId
                            group a by a.ParentId into c
                            select new
                            {
                                ParentId = c.Key,
                            };
                var listMenuCDep = tables.Where(x => x.MenuRank == 1 && exits.Count(e => e.ParentId == x.Id) > 0).Select(a => new
                {
                    a.Name,
                    a.ParentId,
                    a.Id,
                    a.MenuRank,
                    children = tables.Where(x => x.MenuRank == 2 && x.ParentId == a.Id && x.IsActive == true && exits.Count(e => e.ParentId == x.Id) > 0).Select(b => new
                    {
                        b.Name,
                        b.ParentId,
                        b.Id,
                        b.MenuRank,
                        children = tables.Where(x => x.MenuRank == 3 && x.ParentId == b.Id && x.IsActive == true).Select(c => new
                        {
                            c.Name,
                            c.ParentId,
                            c.Id,
                            c.MenuRank,
                            IsActive = (_context.Sys_Cog_MenuNest.FirstOrDefault(x => x.MenuId == c.Id && x.CompanyId == options.companyId && x.DepartmentId == options.nestId && x.IsActive == true) != null) ? true : false
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
        [HttpPost]
        [Route("r2AddDataModelNest")]
        public async Task<ActionResult<IEnumerable<Sys_Cog_MenuNest>>> r2AddDataModelNest(MenuCongtyNest options)
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var menuThree = await _context.Sys_Dm_Menu.FindAsync(options.Id);
                var menuComThree = _context.Sys_Cog_MenuNest.Count(x => x.MenuId == menuThree.Id && x.CompanyId == options.CompanyId && x.DepartmentId == options.NestId);
                if (menuComThree == 0)
                {
                    var menuTwo = await _context.Sys_Dm_Menu.FindAsync(menuThree.ParentId);
                    var menuComTwo = _context.Sys_Cog_MenuNest.Count(x => x.MenuId == menuThree.ParentId && x.CompanyId == options.CompanyId && x.DepartmentId == options.NestId);
                    if (menuComTwo == 0)
                    {
                        var menuOne = await _context.Sys_Dm_Menu.FindAsync(menuTwo.ParentId);
                        var menuComOne = _context.Sys_Cog_MenuNest.Count(x => x.MenuId == menuTwo.ParentId && x.CompanyId == options.CompanyId && x.DepartmentId == options.NestId);
                        if (menuComOne == 0)
                        {
                            Sys_Cog_MenuNest objOne = new Sys_Cog_MenuNest();
                            objOne.MenuId = menuOne.Id;
                            objOne.CompanyId = options.CompanyId;
                            objOne.IsActive = true;
                            objOne.ParentId = null;
                            objOne.DepartmentId = options.NestId;
                            objOne.ParentDepartmentId = options.DepartmentId;
                            objOne.UserUpdateId = userId;
                            objOne.DateUpdate = DateTime.Now;
                            _context.Sys_Cog_MenuNest.Add(objOne);
                            Sys_Cog_MenuNest objTwo = new Sys_Cog_MenuNest();
                            objTwo.MenuId = menuTwo.Id;
                            objTwo.CompanyId = options.CompanyId;
                            objTwo.IsActive = true;
                            objTwo.ParentId = menuOne.Id;
                            objTwo.DepartmentId = options.NestId;
                            objTwo.ParentDepartmentId = options.DepartmentId;
                            objTwo.UserUpdateId = userId;
                            objTwo.DateUpdate = DateTime.Now;
                            _context.Sys_Cog_MenuNest.Add(objTwo);
                            Sys_Cog_MenuNest objThree = new Sys_Cog_MenuNest();
                            objThree.MenuId = options.Id;
                            objThree.CompanyId = options.CompanyId;
                            objThree.IsActive = true;
                            objThree.ParentId = menuTwo.Id;
                            objThree.DepartmentId = options.NestId;
                            objThree.ParentDepartmentId = options.DepartmentId;
                            objThree.UserUpdateId = userId;
                            objThree.DateUpdate = DateTime.Now;
                            _context.Sys_Cog_MenuNest.Add(objThree);
                        }
                        else
                        {
                            Sys_Cog_MenuNest objTwo = new Sys_Cog_MenuNest();
                            objTwo.MenuId = menuTwo.Id;
                            objTwo.CompanyId = options.CompanyId;
                            objTwo.IsActive = true;
                            objTwo.ParentId = menuOne.Id;
                            objTwo.DepartmentId = options.NestId;
                            objTwo.ParentDepartmentId = options.DepartmentId;
                            objTwo.UserUpdateId = userId;
                            objTwo.DateUpdate = DateTime.Now;
                            _context.Sys_Cog_MenuNest.Add(objTwo);
                            Sys_Cog_MenuNest objThree = new Sys_Cog_MenuNest();
                            objThree.MenuId = options.Id;
                            objThree.CompanyId = options.CompanyId;
                            objThree.ParentId = menuTwo.Id;
                            objThree.IsActive = true;
                            objThree.DepartmentId = options.NestId;
                            objThree.ParentDepartmentId = options.DepartmentId;
                            objThree.UserUpdateId = userId;
                            objThree.DateUpdate = DateTime.Now;
                            _context.Sys_Cog_MenuNest.Add(objThree);
                        }
                    }
                    else
                    {
                        Sys_Cog_MenuNest objThree = new Sys_Cog_MenuNest();
                        objThree.MenuId = options.Id;
                        objThree.CompanyId = options.CompanyId;
                        objThree.IsActive = true;
                        objThree.ParentId = menuTwo.Id;
                        objThree.DepartmentId = options.NestId;
                        objThree.ParentDepartmentId = options.DepartmentId;
                        objThree.UserUpdateId = userId;
                        objThree.DateUpdate = DateTime.Now;
                        _context.Sys_Cog_MenuNest.Add(objThree);
                        if (_context.Sys_Cog_MenuNest.Count(x => x.IsActive == true && x.ParentId == menuThree.ParentId && x.CompanyId == options.CompanyId && x.MenuId != options.Id && x.DepartmentId == options.NestId) == 0)
                        {
                            var menuTwoParent = await _context.Sys_Cog_MenuNest.FirstOrDefaultAsync(x => x.MenuId == menuThree.ParentId && x.CompanyId == options.CompanyId && x.DepartmentId == options.NestId);
                            menuTwoParent.IsActive = true;
                            if (_context.Sys_Cog_MenuNest.Count(x => x.IsActive == true && x.ParentId == menuTwoParent.ParentId && x.CompanyId == options.CompanyId && x.MenuId != menuTwoParent.MenuId && x.DepartmentId == options.NestId) == 0)
                            {
                                var menuoneParent = await _context.Sys_Cog_MenuNest.FirstOrDefaultAsync(x => x.MenuId == menuTwoParent.ParentId && x.CompanyId == options.CompanyId && x.DepartmentId == options.NestId);
                                menuoneParent.IsActive = true;
                            }
                        }
                    }

                }
                else
                {
                    var menuCome = await _context.Sys_Cog_MenuNest.FirstOrDefaultAsync(x => x.MenuId == options.Id && x.CompanyId == options.CompanyId && x.DepartmentId == options.NestId);
                    if (options.IsActive == false)
                    {
                        if (_context.Sys_Cog_MenuNest.Count(x => x.IsActive == true && x.ParentId == menuCome.ParentId && x.CompanyId == options.CompanyId && x.MenuId != options.Id && x.DepartmentId == options.NestId) == 0)
                        {
                            var menuTwoParent = await _context.Sys_Cog_MenuNest.FirstOrDefaultAsync(x => x.MenuId == menuCome.ParentId && x.CompanyId == options.CompanyId && x.DepartmentId == options.NestId);
                            menuTwoParent.IsActive = false;
                            if (_context.Sys_Cog_MenuNest.Count(x => x.IsActive == true && x.ParentId == menuTwoParent.ParentId  && x.CompanyId == options.CompanyId && x.MenuId != menuTwoParent.MenuId && x.DepartmentId == options.NestId) == 0)
                            {
                                var menuoneParent = await _context.Sys_Cog_MenuNest.FirstOrDefaultAsync(x => x.MenuId == menuTwoParent.ParentId && x.CompanyId == options.CompanyId && x.DepartmentId == options.NestId);
                                menuoneParent.IsActive = false;
                            }
                        }
                        menuCome.IsActive = false;
                        menuCome.UserUpdateId = userId;
                        menuCome.DateUpdate = DateTime.Now;
                    }
                    else
                    {
                        if (_context.Sys_Cog_MenuNest.Count(x => x.IsActive == true && x.ParentId == menuCome.ParentId && x.CompanyId == options.CompanyId && x.MenuId != options.Id && x.DepartmentId == options.NestId) == 0)
                        {
                            var menuTwoParent = await _context.Sys_Cog_MenuNest.FirstOrDefaultAsync(x => x.MenuId == menuCome.ParentId && x.CompanyId == options.CompanyId && x.DepartmentId == options.NestId);
                            menuTwoParent.IsActive = true;
                            if (_context.Sys_Cog_MenuNest.Count(x => x.IsActive == true && x.ParentId == menuTwoParent.ParentId && x.CompanyId == options.CompanyId && x.MenuId != menuTwoParent.MenuId && x.DepartmentId == options.NestId) == 0)
                            {
                                var menuoneParent = await _context.Sys_Cog_MenuNest.FirstOrDefaultAsync(x => x.MenuId == menuTwoParent.ParentId && x.CompanyId == options.CompanyId && x.DepartmentId == options.NestId);
                                menuoneParent.IsActive = true;
                            }
                        }
                        menuCome.IsActive = true;
                        menuCome.UserUpdateId = userId;
                        menuCome.DateUpdate = DateTime.Now;
                    }
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
    }
}