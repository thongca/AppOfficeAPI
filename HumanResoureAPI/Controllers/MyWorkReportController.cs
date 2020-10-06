using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HumanResource.Data.EF;
using HumanResource.Data.Entities.Works;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HumanResoureAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyWorkReportController : ControllerBase
    {
        private readonly humanDbContext _context;
        public MyWorkReportController(humanDbContext context)
        {
            _context = context;
        }
        #region Báo cáo tính KPI tháng của nhân viên
        // Post: api/MyWorkReport/r1EvalueKPIOneUser
        [HttpGet]
        [Route("r1EvalueKPIOneUser")]
        public async Task<ActionResult<IEnumerable<CV_QT_MyWork>>> r1EvalueKPIOneUser()
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var userID = new SqlParameter("@userID", userId);
                var reports = _context.RePort_KpiForUseraMonth.FromSqlRaw("EXEC RePort_KPIForEmployeeaMonth @userID", userID).ToList();

                return new ObjectResult(new { error = 0, data = reports });
                

            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Báo cáo nhật ký công việc
        // Post: api/MyWorkReport/r1ReportNoteWorks
        [HttpGet]
        [Route("r1ReportNoteWorks")]
        public async Task<ActionResult<IEnumerable<CV_QT_MyWork>>> r1ReportNoteWorks()
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var reports = from a in _context.CV_QT_WorkNote
                              join b in _context.CV_QT_MyWork on a.MyWorkId equals b.Id
                              where a.CreatedBy == userId
                              select new
                              {
                                  a.Id,
                                  a.MyWorkId,
                                  a.WorkTime,
                                  a.DateStart,
                                  a.DateEnd,
                                  a.CreatedBy,
                                  b.TaskCode,
                                  b.TaskName
                              };
                var qrs =await reports.OrderBy(x => x.DateStart).ToListAsync();
                return new ObjectResult(new { error = 0, data = qrs });


            }
            catch (Exception e)
            {
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
    }
}