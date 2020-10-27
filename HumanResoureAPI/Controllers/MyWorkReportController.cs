using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using HumanResource.Application.Paremeters.Works;
using HumanResource.Data.EF;
using HumanResource.Data.Entities.Works;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Spire.Xls;

namespace HumanResoureAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyWorkReportController : ControllerBase
    {
        private readonly humanDbContext _context;
        private IHostingEnvironment _hostingEnvironment;
        public MyWorkReportController(humanDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
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
        #region Bao cao tinh tong thoi gian lam viec co luy ke
        // Post: api/MyWorkReport/r1EvalueReportTotalTime
        [HttpPost]
        [Route("r1EvalueReportTotalTime")]
        public async Task<ActionResult<IEnumerable<CV_QT_MyWork>>> r1EvalueReportTotalTime(Report_TotalTimePara report)
        {
            try
            {
                var dates = new SqlParameter("@dates", report.dates);
                var datee = new SqlParameter("@dates", report.datee);
                var datef = new SqlParameter("@datef", new DateTime(DateTime.Now.Year, 01, 01));
                var reports = _context.Report_TotalTimeWork.FromSqlRaw("EXEC Report_TotalTimeWork {0}, {1}, {2}", report.dates, report.datee, new DateTime(DateTime.Now.Year, 01, 01)).ToList();

                return new ObjectResult(new { error = 0, data = reports });


            }
            catch (Exception e)
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
        #region Xuất Excel hiệu quả công việc
        [HttpPost]
        [Route("r1ExcelKpi")]
        public async Task<IActionResult> ExportExcel()
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var user =await _context.Sys_Dm_User.FindAsync(userId);
                string folderImport = "Resources\\ExportFile\\ParaFile\\";
                string folderExport = "Resources\\ExportFile\\HieuQuaCV\\";
                var ImPath = Path.Combine(Directory.GetCurrentDirectory(), folderImport);
                var ExPath = Path.Combine(Directory.GetCurrentDirectory(), folderExport);
                string fullPath = "";
                string fullPathEx = "";
                Workbook workbook = new Workbook(); // import
                Workbook wbex = new Workbook(); // export
                if (Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files[0];
                    string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).Name.Trim('"');
                    string Pathl = ImPath + fileName;
                    fullPath = ImPath + fileName;
                    using (var stream = new FileStream(Pathl, FileMode.Create))
                    {

                        file.CopyTo(stream);

                    }
                   
                }
                workbook.LoadFromFile(fullPath);
                workbook.SaveToFile(fullPath, ExcelVersion.Version2013);
                #region Xuất báo cáo nhật ký công việc
                fullPathEx = ExPath + "KPI.xlsx";

                StringBuilder sb = new StringBuilder();
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                FileInfo temp = new FileInfo(fullPath);
                FileInfo tempEx = new FileInfo(fullPathEx);
                using (ExcelPackage pck = new ExcelPackage(temp))
                {
                    ExcelWorksheet wsIm = pck.Workbook.Worksheets.FirstOrDefault();
                    using (ExcelPackage pckex = new ExcelPackage(tempEx))
                    {
                        ExcelWorksheet ws = pckex.Workbook.Worksheets.FirstOrDefault();
                        ws.Cells[3, 1].Value = "Họ và tên: " + user.FullName.ToUpper() + "  - Tháng 10  năm 2020";
                        for (int r = 1; r < wsIm.Dimension.End.Row; r++)
                        {
                            for (int c = 1; c <= wsIm.Dimension.End.Column; c++)
                            {
                                ws.Cells[r + 6, c].Value = wsIm.Cells[r, c].Value != null ? wsIm.Cells[r, c].Value.ToString() : "";
                            }
                          
                        }
                       
                        byte[] fileContents;
                        string ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        //Dispose the Excel engine
                        fileContents = pckex.GetAsByteArray();
                        return File(
                                    fileContents: fileContents,
                                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                    fileDownloadName: "test.xlsx"
                                    );
                    }
                     

                }
                
                #endregion 
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
        #region Xuất Excel nhật ký công việc
        [HttpPost]
        [Route("ExportNkCvExcel")]
        public async Task<IActionResult> ExportNkCvExcel()
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var user = await _context.Sys_Dm_User.FindAsync(userId);
                string folderImport = "Resources\\ExportFile\\ParaFile\\";
                string folderExport = "Resources\\ExportFile\\HieuQuaCV\\";
                var ImPath = Path.Combine(Directory.GetCurrentDirectory(), folderImport);
                var ExPath = Path.Combine(Directory.GetCurrentDirectory(), folderExport);
                string fullPath = "";
                string fullPathEx = "";
                Workbook workbook = new Workbook(); // import
                Workbook wbex = new Workbook(); // export
                if (Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files[0];
                    string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).Name.Trim('"');
                    string Pathl = ImPath + fileName;
                    fullPath = ImPath + fileName;
                    using (var stream = new FileStream(Pathl, FileMode.Create))
                    {

                        file.CopyTo(stream);

                    }

                }
                workbook.LoadFromFile(fullPath);
                #region Xuất báo cáo nhật ký công việc
                fullPathEx = ExPath + "nkcv.xlsx";

                StringBuilder sb = new StringBuilder();
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                FileInfo temp = new FileInfo(fullPath);
                FileInfo tempEx = new FileInfo(fullPathEx);
                using (ExcelPackage pck = new ExcelPackage(temp))
                {
                    ExcelWorksheet wsIm = pck.Workbook.Worksheets.FirstOrDefault();
                    using (ExcelPackage pckex = new ExcelPackage(tempEx))
                    {
                        ExcelWorksheet ws = pckex.Workbook.Worksheets.FirstOrDefault();
                        ws.Cells[3, 1].Value = "Họ và tên: " + user.FullName.ToUpper() + " - Từ ngày …../…/… đến ngày …./..../…...";
                        for (int r = 1; r <= wsIm.Dimension.End.Row; r++)
                        {
                            for (int c = 1; c <= wsIm.Dimension.End.Column; c++)
                            {
                                ws.Cells[r + 5, c].Value = wsIm.Cells[r, c].Value != null ? wsIm.Cells[r, c].Value.ToString() : "";
                            }

                        }

                        byte[] fileContents;
                        string ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        //Dispose the Excel engine
                        fileContents = pckex.GetAsByteArray();
                        return File(
                                    fileContents: fileContents,
                                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                    fileDownloadName: "test.xlsx"
                                    );
                    }


                }

                #endregion
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
    }
}