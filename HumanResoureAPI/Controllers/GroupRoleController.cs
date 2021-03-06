﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class GroupRoleController : ControllerBase
    {
        private readonly humanDbContext _context;
        public GroupRoleController(humanDbContext context)
        {
            _context = context;
        }
        #region Danh sách nhóm quyền
        // Post: api/GroupRole/r1GetListGroupRole
        [HttpPost]
        [Route("r1GetListGroupRole")]
        public async Task<ActionResult<IEnumerable<Sys_Dm_GroupRole>>> r1GetListGroupRole(OptionNull options)
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var tables = _context.Sys_Dm_GroupRole.Select(a => new
                {
                    a.Name,
                    a.Id,
                    a.IsOrder,
                    a.IsAdminCom,
                    a.IsAdminDep,
                    a.IsAdminNest,
                    a.IsActive,
                    a.DepartmentId,
                    a.CompanyId
                }).AsQueryable();
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
        // Post: api/GroupRole/r1AddDataGroupRole
        [HttpPost]
        [Route("r1AddDataGroupRole")]
        public async Task<ActionResult<IEnumerable<Sys_Dm_GroupRole>>> r1AddDataGroupRole(Sys_Dm_GroupRole sys_Dm_Group)
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                sys_Dm_Group.UserCreateId = userId;
                sys_Dm_Group.CreateDate = DateTime.Now;
                sys_Dm_Group.RankRole = GetRankRole(sys_Dm_Group);
                _context.Sys_Dm_GroupRole.Add(sys_Dm_Group);
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0 });

            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }
        }
        // GET: api/GroupRole/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Sys_Dm_GroupRole>> GetSys_Dm_GroupRole(int id)
        {
            try
            {
                var sys_Dm_GroupRole = await _context.Sys_Dm_GroupRole.FindAsync(id);

                if (sys_Dm_GroupRole == null)
                {
                    return new ObjectResult(new { error = 1 });
                }

                return new ObjectResult(new { error = 0, data = sys_Dm_GroupRole });
            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }
        }

        // PUT: api/GroupRole/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSys_Dm_GroupRole(int id, Sys_Dm_GroupRole sys_Dm_Group)
        {
            if (id != sys_Dm_Group.Id)
            {
                return new ObjectResult(new { error = 1 });
            }
            sys_Dm_Group.RankRole = GetRankRole(sys_Dm_Group);
            _context.Entry(sys_Dm_Group).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return new ObjectResult(new { error = 1 });
            }

            return new ObjectResult(new { error = 0 });
        }
        // DELETE: api/GroupRole/5
        [HttpPost]
        [Route("r4DelSys_Dm_GroupRole")]
        public async Task<IActionResult> r4DelUsers([FromBody] List<Sys_Dm_GroupRole> listDataRms)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { error = 1 });
            }
            var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
            foreach (var item in listDataRms)
            {
                if (_context.Sys_Cog_UsersGroup.Count(x => x.GroupRoleId == item.Id) > 0)
                {
                    continue;
                }
                var listDataRm = _context.Sys_Dm_GroupRole.FirstOrDefault(x => x.Id == item.Id);
                if (listDataRm == null)
                {
                    return new JsonResult(new { error = 1, ms = "Lỗi khi xóa dữ liệu. Vui lòng thử lại!" });
                }

                _context.Sys_Dm_GroupRole.Remove(listDataRm);
            }
            await _context.SaveChangesAsync();
            return new JsonResult(new { error = 0 });

        }
        private int GetRankRole(Sys_Dm_GroupRole sys_Dm_GroupRole)
        {
            if (sys_Dm_GroupRole.IsAdminCom == true)
            {
                return 1;
            }
            else if (sys_Dm_GroupRole.IsAdminDep == true)
            {
                return 2;
            }
            else if (sys_Dm_GroupRole.IsAdminNest == true)
            {
                return 3;
            }
            else
            {
                return 4;
            }
        }
    }
}