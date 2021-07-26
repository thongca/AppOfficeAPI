using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HumanResource.Data.EF;
using HumanResource.Data.Entities.Works;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HumanResoureAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyWorkErrorController : ControllerBase
    {
        private readonly humanDbContext _context;
        public MyWorkErrorController(humanDbContext context)
        {
            _context = context;
        }
        #region Thêm danh mục lỗi đánh giá chất lượng
        // Post: api/MyWorkError/r2PostAddWorkCV_DM_Error
        [HttpPost]
        [Route("r2PostAddWorkCV_DM_Error")]
        public async Task<ActionResult<IEnumerable<CV_DM_Error>>> r2PostAddWorkCV_DM_Error(CV_DM_Error data)
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var user = await _context.Sys_Dm_User.FindAsync(userId);
                data.DepartmentId = user.DepartmentId ?? 0;
                _context.CV_DM_Error.Add(data);
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0, ms = "Thêm mới lỗi đánh giá công việc thành công!" });
            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1, ms = "Thêm mới lỗi đánh giá công việc không thành công!" });
            }


        }
        #endregion
        #region Danh sách lỗi đánh giá chất lượng
        // Get: api/MyWorkError/r1getListCV_DM_Error
        [HttpPost]
        [Route("r1getListCV_DM_Error")]
        public async Task<ActionResult<IEnumerable<CV_DM_Error>>> r1getListCV_DM_Error()
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var user = await _context.Sys_Dm_User.FindAsync(userId);
                var datas = await _context.CV_DM_Error.Where(x => x.DepartmentId == user.DepartmentId && x.Active != true).Select(a => new
                {
                    a.ErrorName,
                    a.Id,
                    a.Point,
                }).ToListAsync();
                return new ObjectResult(new { error = 0, data = datas });
            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1, ms = "Lấy danh sách lỗi đánh giá chất lượng không thành công!" });
            }


        }
        #endregion
        #region Danh mục lỗi đánh giá chất lượng theo ID
        // Get: api/MyWorkError/5
        [HttpGet("{Id}")]
        public async Task<ActionResult<IEnumerable<CV_DM_Error>>> r1getCV_DM_ErrorByID(int Id)
        {
            try
            {
                var data = await _context.CV_DM_Error.FindAsync(Id);
                if (data == null)
                {
                    return NoContent();
                }
                return new ObjectResult(new { error = 0, data });
            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1, ms = "Lấy dữ liệu theo Id không thành công!" });
            }


        }
        #endregion
        #region Cập nhật  danh mục lỗi đánh giá chất lượng
        // Get: api/MyWorkError/5
        [HttpPost]
        public async Task<ActionResult<IEnumerable<CV_DM_Error>>> r3UpdateWorkDefaultByID(CV_DM_Error dM_Error)
        {
            try
            {
                var error = await _context.CV_DM_Error.FindAsync(dM_Error.Id);
                if (error == null)
                {
                    return new ObjectResult(new { error = 1, ms = "Cập nhật danh mục lỗi đánh giá không thành công!" });
                }
                error.ErrorName = dM_Error.ErrorName;
                error.Point = dM_Error.Point;
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0, ms = "Cập nhật danh mục lỗi đánh giá thành công!" });
            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1, ms = "Cập nhật danh mục lỗi đánh giá không thành công!" });
            }


        }
        #endregion
        // Post: api/MyWorkError/r4DelCV_DM_Error
        [HttpPost]
        [Route("r4DelCV_DM_Error")]
        public async Task<IActionResult> r4DelCV_DM_Error(List<CV_DM_Error> listDataRms)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { error = 1, ms = "Xóa danh mục lỗi không thành công!" });
            }
            _context.CV_DM_Error.RemoveRange(listDataRms);
            await _context.SaveChangesAsync();
            return new JsonResult(new { error = 0 });

        }
    }
}
