using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using HumanResource.Application.Helper.Dtos;
using HumanResource.Application.Paremeters;
using HumanResource.Application.Paremeters.Works;
using HumanResource.Data.EF;
using HumanResource.Data.Entities.Works;
using HumanResoureAPI.Common;
using HumanResoureAPI.Common.WorksCommon;
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
        public async Task<ActionResult<IEnumerable<CV_QT_MyWork>>> r1EvalueKPIOneUser(Report_TotalTimePara optionRePort)
        {
            try
            {
                var userId = 0;
                if (optionRePort.UserId == 0)
                {
                    userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                }
                else
                {
                    userId = optionRePort.UserId;
                }
                var datesDefault = TransforDate.FromDateToDouble(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01));
                var dateeDefault = TransforDate.FromDateToDouble(DateTime.Now);
                var reports = await _context.RePort_KpiForUseraMonth.FromSqlRaw("EXEC RePort_KPIForEmployeeaMonth {0}, {1}, {2}", userId,
                    TransforDate.FromDoubleToDate(optionRePort.dates ?? datesDefault),
                    TransforDate.FromDoubleToDate(optionRePort.datee ?? dateeDefault)
                    ).ToListAsync();

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
                    TransforDate.FromDoubleToDate(report.dates ?? datesDefault),
                    TransforDate.FromDoubleToDate(report.datee ?? dateeDefault),
                    new DateTime(DateTime.Now.Year, 01, 01)).ToList();

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
        [HttpPost]
        [Route("r1ReportNoteWorks")]
        public async Task<ActionResult<IEnumerable<CV_QT_MyWork>>> r1ReportNoteWorks(Report_TotalTimePara model)
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

                var datesDefault = TransforDate.FromDateToDouble(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01));
                var dateeDefault = TransforDate.FromDateToDouble(DateTime.Now);
                var reports = from a in _context.CV_QT_WorkNote
                              join b in _context.CV_QT_MyWork on a.MyWorkId equals b.Id
                              where a.CreatedBy == userId
                              && a.DateStart.Value.Date >= TransforDate.FromDoubleToDate(model.dates ?? datesDefault).Date
                              && a.DateStart.Value.Date <= TransforDate.FromDoubleToDate(model.datee ?? dateeDefault).Date
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
                var qrs = await reports.OrderBy(x => x.DateStart).ToListAsync();
                return new ObjectResult(new { error = 0, data = qrs });


            }
            catch (Exception e)
            {
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Biểu đồ thời gian làm việc của nhân việc trong 1 phòng
        // Post: api/MyWorkReport/r1WorkTimeForDepartment
        [HttpGet]
        [Route("r1WorkTimeForDepartment")]
        public async Task<ActionResult<IEnumerable<CV_QT_MyWork>>> r1WorkTimeForDepartment()
        {
            try
            {
                 RequestToken token = CommonData.GetDataFromToken(User);
                var user =await _context.Sys_Dm_User.FindAsync(token.UserID);
                var listUser =await _context.Sys_Dm_User.Where(x => x.ParentDepartId == user.ParentDepartId).Select(x => x.Id).ToListAsync();
                var datesDefault = TransforDate.FromDateToDouble(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01));
                var dateeDefault = TransforDate.FromDateToDouble(DateTime.Now);
                var reports = from a in _context.CV_QT_MyWork
                              join b in _context.Sys_Dm_User on a.UserTaskId equals b.Id
                              where a.EndDate.Value.Date >= TransforDate.FromDoubleToDate(datesDefault).Date
                              && a.EndDate.Value.Date <= TransforDate.FromDoubleToDate(dateeDefault).Date
                              && listUser.Contains(a.UserTaskId)
                              group a by new {a.UserTaskId, b.FullName} into gr
                              select new
                              {
                                  gr.Key.UserTaskId,
                                  gr.Key.FullName,
                                  PointCount = gr.Sum(x=>x.WorkTime)

                              };
                var qrs = await reports.OrderBy(x => x.FullName).ToListAsync();
                return new ObjectResult(new { error = 0, data = qrs });


            }
            catch (Exception e)
            {
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Thời gian trống trong ngày
        // Post: api/MyWorkReport/r1SpaceTimeOnDay
        [HttpGet]
        [Route("r1SpaceTimeOnDay")]
        public async Task<ActionResult<IEnumerable<CV_QT_MyWork>>> r1SpaceTimeOnDay()
        {
            try
            {
                TimeSpan ts17 = new TimeSpan(17, 00, 0);
                 RequestToken token = CommonData.GetDataFromToken(User);

                var datesDefault = TransforDate.FromDateToDouble(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01));
                var dateeDefault = TransforDate.FromDateToDouble(DateTime.Now);
                var reports = (from a in _context.CV_QT_StartPauseHistory
                               where a.UserCreateId == token.UserID && a.Done != true && a.CreateDate.Month == DateTime.Now.Month
                               orderby a.Id
                               select new
                               {
                                   a.Id,
                                   a.CreateDate,
                                   a.CycleWork,
                                   a.MyWorkId,
                                   a.UserCreateId
                               }).ToList();
                if (reports.Count() < 2)
                {
                    return new ObjectResult(new { error = 0 });
                }
                List<CV_QT_SpaceTimeOnDay> listSpace = new List<CV_QT_SpaceTimeOnDay>();

                for (int i = 0; i < reports.Count() - 1; i++)
                {

                    if (reports[i].CreateDate.Hour >=17 && reports[i].CreateDate.Minute > 3)
                    {
                        continue;
                    }
                   if (reports[i].CycleWork == 2)
                    {
                        CV_QT_SpaceTimeOnDay obj = new CV_QT_SpaceTimeOnDay()
                        {
                            SpaceStart = TransforDate.FromDateToDouble(reports[i].CreateDate),
                            SpaceEnd = TransforDate.FromDateToDouble(reports[i + 1].CreateDate),
                            Time = SpaceTimeOnDay.CalSpaceTimeOnDay(reports[i].CreateDate, reports[i + 1].CreateDate),
                            MyWorkId = reports[i].MyWorkId,
                            UserId = reports[i].UserCreateId
                        };
                        if (obj.Time >= 30)
                        {
                            listSpace.Add(obj);
                        }
                    }
                }
                var objup =  _context.CV_QT_StartPauseHistory.Where(x=>x.UserCreateId == token.UserID && x.Done != true);
                foreach (var item in objup)
                {
                    item.Done = true;

                }
                _context.CV_QT_StartPauseHistory.UpdateRange(objup);
                await _context.AddRangeAsync(listSpace);
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0 });


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
                var datesDefault = TransforDate.FromDateToDouble(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01));
                var dateeDefault = TransforDate.FromDateToDouble(DateTime.Now);
                var startDate = TransforDate.FromDoubleToDate(model.ReportDate.dates ?? datesDefault);
                var endDate = TransforDate.FromDoubleToDate(model.ReportDate.datee ?? dateeDefault);
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
                        ws.Cells[3, 1].Value = "Họ và tên: " + user.FullName.ToUpper() + "Từ "+ startDate.ToString("dd/MM/yyyy") + " đến " + endDate.ToString("dd/MM/yyyy");
                        for (int r = 1; r < wsIm.Dimension.End.Row; r++)
                        {
                            for (int c = 1; c <= wsIm.Dimension.End.Column; c++)
                            {
                                ws.Cells[r + 6, c].Value = wsIm.Cells[r, c].Value != null ? wsIm.Cells[r, c].Value : "";
                                if (c == 8 || c == 18)
                                {
                                    ws.Cells[r + 6, c].Value = wsIm.Cells[r, c].Value != null ? Convert.ToDouble(wsIm.Cells[r, c].Value) / 100 : 0;
                                }
                               
                            }
                            ws.InsertRow(r + 7, 1, 7);
                            if (r == wsIm.Dimension.End.Row - 1)
                            {
                                ws.SelectedRange[r + 8, 7].Value = "=Sum(G7:G" + (wsIm.Dimension.End.Row + 5) + ")";
                                ws.SelectedRange[r + 8, 7].Formula = "=Sum(G7:G" + (wsIm.Dimension.End.Row + 5) + ")";
                                ws.SelectedRange[r + 8, 8].Value = "=Sum(H7:H" + (wsIm.Dimension.End.Row + 5) + ")";
                                ws.SelectedRange[r + 8, 8].Formula = "=Sum(H7:H" + (wsIm.Dimension.End.Row + 5) + ")";
                                ws.SelectedRange[r + 8, 17].Value = "=Sum(R7:R" + (wsIm.Dimension.End.Row + 5) + ")";
                                ws.SelectedRange[r + 8, 17].Formula = "=Sum(Q7:Q" + (wsIm.Dimension.End.Row + 5) + ")";
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

                var datesDefault = TransforDate.FromDateToDouble(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01));
                var dateeDefault = TransforDate.FromDateToDouble(DateTime.Now);
                var startDate = TransforDate.FromDoubleToDate(model.ReportDate.dates?? datesDefault);
                var endDate = TransforDate.FromDoubleToDate(model.ReportDate.datee?? dateeDefault);
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
                        ws.Cells[3, 1].Value = "Họ và tên: " + user.FullName.ToUpper() + " - Từ ngày "+ startDate.ToString("dd/MM/yy") + " đến ngày " + endDate.ToString("dd/MM/yy");
                        for (int r = 1; r <= wsIm.Dimension.End.Row; r++)
                        {
                            
                            for (int c = 1; c <= wsIm.Dimension.End.Column; c++)
                            {
                               
                                ws.Cells[r + 5, c].Value = wsIm.Cells[r, c].Value != null ? wsIm.Cells[r, c].Value : "";
                            }
                            ws.InsertRow(r + 6, 1, 6);
                            if (r  == wsIm.Dimension.End.Row)
                            {
                                ws.SelectedRange[r + 7, 8].Value = "=Sum(H6:H"+ (wsIm.Dimension.End.Row + 5)+")";
                                ws.SelectedRange[r + 7, 8].Formula = "=Sum(H6:H"+ (wsIm.Dimension.End.Row + 5)+")";
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
            catch (Exception)
            {
                return NoContent();
            }
        }
        #endregion
        #region Xuất Excel tổng hợp công việc
        [HttpPost]
        [Route("ExportTotalWorkExcel")]
        public async Task<IActionResult> ExportTotalWorkExcel()
        {
            try
            {
                var model = JsonConvert.DeserializeObject<Report_TotalTimePara>(Request.Form["model"]);
                var userId = 0;
                if (model.UserId == 0) // nếu user truyền vào = 0 thì gán cho user mặc định
                {
                    userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                }
                else
                {
                    userId = model.UserId;
                }

                var datesDefault = TransforDate.FromDateToDouble(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01));
                var dateeDefault = TransforDate.FromDateToDouble(DateTime.Now);
                var startDate = TransforDate.FromDoubleToDate(model.dates ?? datesDefault);
                var endDate = TransforDate.FromDoubleToDate(model.datee ?? dateeDefault);
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
                #region Xuất báo cáo tổng hợp công việc
                fullPathEx = ExPath + "Sumtime.xlsx";

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
                        ws.Cells[4, 4].Value = "Từ ngày" + startDate.Date.ToString("dd/MM/yyyy") + "đến hết ngày" + endDate.Date.ToString("dd/MM/yyyy");
                        for (int r = 1; r <= wsIm.Dimension.End.Row; r++)
                        {

                            for (int c = 1; c <= wsIm.Dimension.End.Column; c++)
                            {

                                ws.Cells[r + 6, c].Value = wsIm.Cells[r, c].Value != null ? wsIm.Cells[r, c].Value : "";
                            }
                           

                        }

                        byte[] fileContents;
                        string ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        //Dispose the Excel engine
                        fileContents = pckex.GetAsByteArray();
                        return File(
                                    fileContents: fileContents,
                                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                    fileDownloadName: "baocao.xlsx"
                                    );
                    }


                }

                #endregion
            }
            catch (Exception)
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
                        ws.Cells[r + 7, 4].Value = Math.Round(Convert.ToDouble(reports[r].Tong / report.TotalHour ?? 1), 2);
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
        #region Tính lại thời gian nhật ký công việc tạm thời
        //api: api/MyWorkReport/TinhLaiNhatKy
        [HttpGet]
        [Route("TinhLaiNhatKy")]
        public async Task<IActionResult> TinhLaiNhatKy()
        {
            try
            {
                var or = _context.CV_QT_WorkNote.Where(x => x.DateEnd != null).ToList();
                foreach (var item in or)
                {
                    item.WorkTime = SpaceTimeOnDay.CalSpaceTimeOnDay(item.DateStart??DateTime.Now, item.DateEnd ?? DateTime.Now) / 60;
                }
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0 });
            }
            catch (Exception)
            {
                return NoContent();
            }
        }
        #endregion
    }
}