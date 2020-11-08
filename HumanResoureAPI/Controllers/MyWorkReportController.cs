using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using HumanResource.Application.Paremeters;
using HumanResource.Application.Paremeters.Works;
using HumanResource.Data.EF;
using HumanResource.Data.Entities.Works;
using HumanResoureAPI.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
        [HttpPost]
        [Route("r1EvalueKPIOneUser")]
        public async Task<ActionResult<IEnumerable<CV_QT_MyWork>>> r1EvalueKPIOneUser(OptionRePort optionRePort)
        {
            try
            {
                var userId = 0;
                if (optionRePort.UserId == 0)
                {
                    userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                } else
                {
                    userId = optionRePort.UserId;
                }
                 var userID = new SqlParameter("@userID", userId);
                var reports =await _context.RePort_KpiForUseraMonth.FromSqlRaw("EXEC RePort_KPIForEmployeeaMonth @userID", userID).ToListAsync();

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
                var datesDefault = TransforDate.FromDateToDouble(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01));
                var dateeDefault = TransforDate.FromDateToDouble(DateTime.Now);
                var reports = _context.Report_TotalTimeWork.FromSqlRaw("EXEC Report_TotalTimeWork {0}, {1}, {2}",
                    TransforDate.FromDoubleToDate(report.dates?? datesDefault), 
                    TransforDate.FromDoubleToDate(report.datee?? dateeDefault), 
                    new DateTime(DateTime.Now.Year, 01, 01)).ToList();

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
        [HttpPost]
        [Route("r1ReportNoteWorks")]
        public async Task<ActionResult<IEnumerable<CV_QT_MyWork>>> r1ReportNoteWorks(OptionRePort model)
        {
            try
            {
                var userId = 0;
                if (model.UserId == 0)
                {
                    userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                }
                else
                {
                    userId = model.UserId;
                }
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
                var model = JsonConvert.DeserializeObject<OptionRePort>(Request.Form["model"]);
                var userId = 0;
                if (model.UserId == 0)
                {
                    userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                }
                else
                {
                    userId = model.UserId;
                }
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
                return NoContent();
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
                var model = JsonConvert.DeserializeObject<OptionRePort>(Request.Form["model"]);
                var userId = 0;
                if (model.UserId == 0) // nếu user truyền vào = 0 thì gán cho user mặc định
                {
                    userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                }
                else
                {
                    userId = model.UserId;
                }
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
                return NoContent();
            }
        }
        #endregion
        #region Xuất Excel tổng hợp thời gian
        [HttpPost]
        [Route("ExportSumTimeExcel")]
        public async Task<IActionResult> ExportTotalSumTimeExcel(Report_TotalTimePara report)
        {
            try
            {
                var datesDefault = TransforDate.FromDateToDouble(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01));
                var dateeDefault = TransforDate.FromDateToDouble(DateTime.Now);
                var reports = _context.Report_TotalTimeWork.FromSqlRaw("EXEC Report_TotalTimeWork {0}, {1}, {2}",
                    TransforDate.FromDoubleToDate(report.dates ?? datesDefault),
                    TransforDate.FromDoubleToDate(report.datee ?? dateeDefault),
                    new DateTime(DateTime.Now.Year, 01, 01)).ToList();
                var userId = 0;
                 userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var user = await _context.Sys_Dm_User.FindAsync(userId);
                string folderExport = "Resources\\ExportFile\\HieuQuaCV\\";
                var ExPath = Path.Combine(Directory.GetCurrentDirectory(), folderExport);
                string fullPathEx = "";
                Workbook wbex = new Workbook(); // export
                #region Xuất báo cáo tổng hợp thời gian
                fullPathEx = ExPath + "Sumtime.xlsx";

                StringBuilder sb = new StringBuilder();
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                FileInfo tempEx = new FileInfo(fullPathEx);
                    using (ExcelPackage pckex = new ExcelPackage(tempEx))
                    {
                        ExcelWorksheet ws = pckex.Workbook.Worksheets.FirstOrDefault();
                        for (int r = 0; r < reports.Count(); r++)
                        {

                        ws.Cells[r + 7, 1].Value = (r + 1).ToString();
                        ws.Cells[r + 7, 2].Value = reports[r].TaskName;
                        ws.Cells[r + 7, 3].Value = reports[r].TaskCode;
                        ws.Cells[r + 7, 4].Value = Math.Round(Convert.ToDouble(reports[r].Tong / report.TotalHour??1), 2);
                        ws.Cells[r + 7, 5].Value = Math.Round(Convert.ToDouble(reports[r].WorkNgay), 2);
                        ws.Cells[r + 7, 6].Value = Math.Round(Convert.ToDouble(reports[r].Tong * 100) / report.TotalHour ?? 1, 2);       
                        ws.Cells[r + 7, 7].Value = Math.Round(Convert.ToDouble(reports[r].ChuTri), 2);
                        ws.Cells[r + 7, 8].Value = Math.Round(Convert.ToDouble(reports[r].PhoiHop), 2);
                        ws.Cells[r + 7, 9].Value = Math.Round(Convert.ToDouble((reports[r].TongLk * 100) / report.TotalHourLk ?? 1), 2);
                        ws.Cells[r + 7, 10].Value = Math.Round(Convert.ToDouble(reports[r].TongLk / report.TotalHourLk ?? 1), 2);
                        ws.Cells[r + 7, 11].Value = Math.Round(Convert.ToDouble(reports[r].NgayLk), 2);
                        ws.Cells[r + 7, 12].Value = Math.Round(Convert.ToDouble(reports[r].ChuTriLk), 2);
                        ws.Cells[r + 7, 13].Value = Math.Round(Convert.ToDouble(reports[r].PhoiHopLk), 2);
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

                #endregion
            }
            catch (Exception ex)
            {
                return NoContent();
            }
        }
        #endregion
    }
}