using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HumanResource.Application.Helper;
using HumanResource.Application.Helper.Dtos;
using HumanResource.Application.Paremeters;
using HumanResource.Data.DTO;
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
    public class UserController : ControllerBase
    {
        private readonly humanDbContext _context;
        public UserController(humanDbContext context)
        {
            _context = context;
        }
        #region Danh sách người dùng
        // Post: api/User/r1GetListUser
        [HttpPost]
        [Route("r1GetListUser")]
        public async Task<ActionResult<IEnumerable<Sys_Dm_User>>> r1GetListUser(Options options)
        {
            try
            {
                 RequestToken token = CommonData.GetDataFromToken(User);
                var tables = _context.Sys_Dm_User.Select(a => new {
                    a.FullName,
                    a.Id,
                    a.Username,
                    a.Password,
                    a.IsActive,
                    a.DepartmentId,
                    a.CompanyId,
                    a.NestId,
                    a.Role
                }).AsQueryable();
                if (token.CompanyId == 0)
                {
                    tables = tables.Where(x => x.CompanyId == 0);
                }
                if (token.CompanyId > 0)
                {
                    tables = tables.Where(x => x.CompanyId == token.CompanyId);
                }
                if (options.departmentId > 0)
                {
                    tables = tables.Where(x => x.DepartmentId == options.departmentId);
                }
                if (options.nestId > 0)
                {
                    tables = tables.Where(x => x.NestId == options.nestId);
                }
                if (!string.IsNullOrEmpty(options.s))
                {
                    tables = tables.Where(c => c.FullName.ToUpper().Contains(options.s.ToUpper()));
                }
                var qrs = await tables.OrderBy(x => x.Id).ToListAsync();
                return new ObjectResult(new { error = 0, data = qrs, total= tables.Count() });

            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        // Post: api/User/r1AddDataSysDmUser
        [HttpPost]
        [Route("r1AddDataSysDmUser")]
        public async Task<ActionResult<IEnumerable<Sys_Dm_User>>> r1AddDataSysDmUser(UserGroupRole userGroupRole)
        {
            try
            {
                 RequestToken token = CommonData.GetDataFromToken(User);
                var userLogin = await _context.Sys_Dm_User.FindAsync(token.UserID);
                if (_context.Sys_Dm_User.Count(x=>x.Code == userGroupRole.sys_Dm_User.Code) > 0)
                {
                    return new ObjectResult(new { error = 2, ms = "Mã nhân viên đã tồn tại trong hệ thống." });
                }
                var department = await _context.Sys_Dm_Department.FindAsync(userGroupRole.sys_Dm_User.DepartmentId);
                var nest = await _context.Sys_Dm_Department.FindAsync(userGroupRole.sys_Dm_User.NestId);
                Nullable<int> NestId = null;
                if (nest != null)
                {
                    NestId = nest.Id;
                }
                var position =await _context.Sys_Dm_Position.FindAsync(userGroupRole.sys_Dm_User.PositionId);
                if (position == null)
                {
                    return new ObjectResult(new { error = 2, ms = "Chưa chọn chức vụ cho nhân viên mới." });
                }
                string PasswordEn = Helper.Encrypt(userGroupRole.sys_Dm_User.Username, userGroupRole.sys_Dm_User.Password);
                userGroupRole.sys_Dm_User.CompanyId = token.CompanyId;
                userGroupRole.sys_Dm_User.Password = PasswordEn;
                userGroupRole.sys_Dm_User.PositionName = position.Name;
                userGroupRole.sys_Dm_User.DepartmentName = department.Name;
                userGroupRole.sys_Dm_User.UserCreateId = token.UserID;
                userGroupRole.sys_Dm_User.CreateDate = DateTime.Now;
                userGroupRole.sys_Dm_User.NestId = NestId;
                _context.Sys_Dm_User.Add(userGroupRole.sys_Dm_User);
                await _context.SaveChangesAsync();
                var user = _context.Sys_Dm_User.FirstOrDefault(x=>x.Code == userGroupRole.sys_Dm_User.Code);
                //foreach (var item in userGroupRole.GroupRoles)
                //{
                //    Sys_Cog_UsersGroup obj = new Sys_Cog_UsersGroup();
                //    obj.UserId = user.Id;
                //    obj.GroupRoleId = item.Id;
                //    _context.Sys_Cog_UsersGroup.Add(obj);
                //}
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0 });

            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Sys_Dm_User>> r1GetDataById(int id)
        {
            try
            {
                var sys_Dm_User = await _context.Sys_Dm_User.FindAsync(id);
                var groups =  from a in _context.Sys_Cog_UsersGroup
                              join b in _context.Sys_Dm_GroupRole on a.GroupRoleId equals b.Id
                              where a.UserId == id
                              select new
                              {
                                  b.Id,
                                  b.Name
                              };

                if (sys_Dm_User == null)
                {
                    return new ObjectResult(new { error = 1 });
                }

                return new ObjectResult(new { error = 0, data = sys_Dm_User, userGroups = groups });
            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }
        }

        // PUT: api/User/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        [Route("r3UpdateDataModel")]
        public async Task<IActionResult> r3UpdateDataModel(UserGroupRole userGroupRole)
        {
            string PasswordEn = Helper.Encrypt(userGroupRole.sys_Dm_User.Username, userGroupRole.sys_Dm_User.Password);
             RequestToken token = CommonData.GetDataFromToken(User);
            userGroupRole.sys_Dm_User.UserCreateId = token.UserID;
            userGroupRole.sys_Dm_User.CreateDate = DateTime.Now;
            // update user
            var user =await _context.Sys_Dm_User.FindAsync(userGroupRole.sys_Dm_User.Id);
            if (user.Password != userGroupRole.sys_Dm_User.Password)
            {
                user.Password = PasswordEn;
            }
            user.FullName = userGroupRole.sys_Dm_User.FullName;
            user.Role = userGroupRole.sys_Dm_User.Role;
            user.GroupRoleId = userGroupRole.sys_Dm_User.GroupRoleId;
            // update quyền
            var userGroup = _context.Sys_Cog_UsersGroup.Where(x=>x.UserId == userGroupRole.sys_Dm_User.Id);
            _context.Sys_Cog_UsersGroup.RemoveRange(userGroup);
            //foreach (var item in userGroupRole.GroupRoles)
            //{
            //    Sys_Cog_UsersGroup obj = new Sys_Cog_UsersGroup();
            //    obj.UserId = user.Id;
            //    obj.GroupRoleId = item.Id;
            //    _context.Sys_Cog_UsersGroup.Add(obj);
            //}
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
        // Post: api/User/r4DelSys_Dm_User
        [HttpPost]
        [Route("r4DelSys_Dm_User")]
        public async Task<IActionResult> r4DelUsers([FromBody] List<Sys_Dm_User> listDataRms)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { error = 1 });
            }
             RequestToken token = CommonData.GetDataFromToken(User);
            foreach (var item in listDataRms)
            {
                var userGroup = _context.Sys_Cog_UsersGroup.Where(x => x.UserId == item.Id);
                _context.Sys_Cog_UsersGroup.RemoveRange(userGroup);
            }
            _context.Sys_Dm_User.RemoveRange(listDataRms);
            await _context.SaveChangesAsync();
            return new JsonResult(new { error = 0 });

        }
    }
}