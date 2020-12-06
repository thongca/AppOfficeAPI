using HumanResource.Application.Helper;
using HumanResource.Application.Paremeters.Works;
using HumanResource.Data.DTO;
using HumanResource.Data.EF;
using HumanResource.Data.Entities.Works;
using HumanResoureAPI.Common;
using HumanResoureAPI.Common.Systems;
using HumanResoureAPI.Common.WorksCommon;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace HumanResoureAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyWorkController : ControllerBase
    {
        private readonly humanDbContext _context;
        public MyWorkController(humanDbContext context)
        {
            _context = context;
        }
        #region Thêm công việc Mywork
        //Post: api/MyWork/r2AddDataMywork
        [HttpPost, DisableRequestSizeLimit]
        [Route("r2AddDataMywork")]
        public async Task<ActionResult<IEnumerable<CV_QT_MyWork>>> r2AddDataMywork()
        {
            try
            {
                // trang thái sử dụng
                var myWork = JsonConvert.DeserializeObject<Dtos_MyWork>(Request.Form["model"]);
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var user = await _context.Sys_Dm_User.FindAsync(userId);
                var defaultWork = _context.CV_DM_DefaultTask.FirstOrDefault(x => x.Name == myWork.CV_QT_MyWork.TaskName);
                if (defaultWork != null)
                {
                    myWork.CV_QT_MyWork.TaskId = defaultWork.Id;
                    myWork.CV_QT_MyWork.TaskCode = defaultWork.Code;
                    if (myWork.CV_QT_MyWork.PointTask == 0) // nếu không chọn thay đổi mức độ công việc thì lấy giá trị mặc định trong danh mục công việc mặc định
                    {
                        myWork.CV_QT_MyWork.PointTask = defaultWork.PointTask;
                    }
                    if (myWork.CV_QT_MyWork.PointTime == 0)// nếu không chọn thay đổi độ vội công việc thì lấy giá trị mặc định trong danh mục công việc mặc định
                    {
                        myWork.CV_QT_MyWork.PointTime = defaultWork.PointTime;
                    }
                }
                else
                {
                    myWork.CV_QT_MyWork.TaskCode = "";
                }
                myWork.CV_QT_MyWork.Id = Helper.GenKey();
                myWork.CV_QT_MyWork.StartPause = null;
                myWork.CV_QT_MyWork.StartDate = null;
                myWork.CV_QT_MyWork.EndPause = null;
                myWork.CV_QT_MyWork.CycleWork = 0;
                myWork.CV_QT_MyWork.TypeComplete = 0;
                myWork.CV_QT_MyWork.PauseTime = 0.0;
                myWork.CV_QT_MyWork.WorkTime = 0.0;
                myWork.CV_QT_MyWork.CreatedDate = DateTime.Now;
                myWork.CV_QT_MyWork.Id = Helper.GenKey();
                _context.CV_QT_MyWork.Add(myWork.CV_QT_MyWork);
                // lưu quy trình luân chuyển công việc
                CV_QT_WorkFlow wflow = WorksCommon.objWorkFlow(_context, myWork.CV_QT_MyWork.Id, userId, userId, 0, "CV_MYWORK", null, myWork.CV_QT_MyWork.Note, "", 1);
                wflow.ReadDate = DateTime.Now;
                wflow.Readed = true;
                _context.CV_QT_WorkFlow.Add(wflow);
                // lưu quy trình luân chuyển công việc
                foreach (var item in myWork.CV_QT_MySupportWorks)
                {
                    item.MyWorkId = myWork.CV_QT_MyWork.Id;
                    _context.CV_QT_MySupportWork.Add(item);
                }
                CV_QT_MyScheduleWork myScheduleWork = new CV_QT_MyScheduleWork()
                {

                    TaskName = myWork.CV_QT_MyWork.TaskName,
                    FullName = user.FullName,
                    UserCreateId = userId,
                    UserDeliverId = userId,
                    StartDate = myWork.CV_QT_MyWork.ExpectedDate,
                    EndDate = myWork.CV_QT_MyWork.EndDate,
                    MyWorkId = myWork.CV_QT_MyWork.Id,
                    Predecessor = 0,
                    WorkTime = 0,
                    StatusWork = 0
                };
                _context.CV_QT_MyScheduleWork.Add(myScheduleWork);
                if (Request.Form.Files.Count != 0)
                {
                    foreach (var item in Request.Form.Files)
                    {
                        CV_QT_WorkFlowFile obj = new CV_QT_WorkFlowFile();
                        var file = item;
                        var folderName = Path.Combine("Resources", "WorkFlows", "Myworks");
                        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                        if (!Directory.Exists(pathToSave))
                        {
                            Directory.CreateDirectory(pathToSave);
                        }
                        if (myWork != null)
                        {
                            if (file.Length > 0)
                            {
                                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                                var fullPath = Path.Combine(pathToSave, fileName);
                                var dbPath = Path.Combine(folderName, fileName);
                                obj.Extension = Path.GetExtension(fileName);
                                obj.Path = dbPath;
                                using (var stream = new FileStream(fullPath, FileMode.Create))
                                {
                                    file.CopyTo(stream);
                                }
                            }
                        }
                        obj.Name = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        obj.WorkFlowId = wflow.Id;
                        obj.Size = Convert.ToDouble(file.Length / 1048576);

                        _context.CV_QT_WorkFlowFile.Add(obj);
                    }

                }
                List<CV_QT_DepartmentSupporter> listDep = new List<CV_QT_DepartmentSupporter>();
                foreach (var item in myWork.CV_QT_DepartmentSupporter)
                {
                    CV_QT_DepartmentSupporter _QT_DepartmentSupporter = new CV_QT_DepartmentSupporter()
                    {
                        MyWorkId = myWork.CV_QT_MyWork.Id,
                        DepartmentId = item.DepartmentId,
                        DepartmentName =  _context.Sys_Dm_Department.Find(item.DepartmentId) != null ? _context.Sys_Dm_Department.Find(item.DepartmentId).Name: null
                    };
                    listDep.Add(_QT_DepartmentSupporter);
                }
                _context.CV_QT_DepartmentSupporter.AddRange(listDep);
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0 });

            }
            catch (Exception e)
            {
                bool success = SaveLog.SaveLogEx(_context, "api/MyWork/r2AddDataMywork", e.Message, "Thêm mới công việc");
                return new ObjectResult(new { error = 1, ms = "Lỗi khi thêm mới công việc!" });
            }
        }
        #endregion
        #region Thêm công việc Mywork khởi tạo sau
        //Post: api/MyWork/r2AddDataMyworkOldTime
        [HttpPost, DisableRequestSizeLimit]
        [Route("r2AddDataMyworkOldTime")]
        public async Task<ActionResult<IEnumerable<CV_QT_MyWork>>> r2AddDataMyworkOldTime()
        {
            try
            {
                // trang thái sử dụng
                var myWork = JsonConvert.DeserializeObject<Dtos_MyWork>(Request.Form["model"]);
                var spaceTime = await _context.CV_QT_SpaceTimeOnDay.FindAsync(myWork.SpaceTimeId);
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var user = await _context.Sys_Dm_User.FindAsync(userId);
                var defaultWork = _context.CV_DM_DefaultTask.FirstOrDefault(x => x.Name == myWork.CV_QT_MyWork.TaskName);
                if (defaultWork != null)
                {
                    myWork.CV_QT_MyWork.TaskId = defaultWork.Id;
                    myWork.CV_QT_MyWork.TaskCode = defaultWork.Code;
                    if (myWork.CV_QT_MyWork.PointTask == 0) // nếu không chọn thay đổi mức độ công việc thì lấy giá trị mặc định trong danh mục công việc mặc định
                    {
                        myWork.CV_QT_MyWork.PointTask = defaultWork.PointTask;
                    }
                    if (myWork.CV_QT_MyWork.PointTime == 0)// nếu không chọn thay đổi độ vội công việc thì lấy giá trị mặc định trong danh mục công việc mặc định
                    {
                        myWork.CV_QT_MyWork.PointTime = defaultWork.PointTime;
                    }
                }
                else
                {
                    myWork.CV_QT_MyWork.TaskCode = "";
                }
                myWork.CV_QT_MyWork.Id = Helper.GenKey();
                myWork.CV_QT_MyWork.StartPause = null;
                myWork.CV_QT_MyWork.StartDate = TransforDate.FromDoubleToDate(spaceTime.SpaceStart);
                myWork.CV_QT_MyWork.EndPause = TransforDate.FromDoubleToDate(spaceTime.SpaceEnd);
                myWork.CV_QT_MyWork.CycleWork = 4;
                myWork.CV_QT_MyWork.TypeComplete = 1;
                myWork.CV_QT_MyWork.PauseTime = 0.0;
                myWork.CV_QT_MyWork.WorkTime = spaceTime.Time / 60;
                myWork.CV_QT_MyWork.ExpectedDate = TransforDate.FromDoubleToDate(spaceTime.SpaceStart);
                myWork.CV_QT_MyWork.EndDate = TransforDate.FromDoubleToDate(spaceTime.SpaceEnd);
                myWork.CV_QT_MyWork.CompleteDate = TransforDate.FromDoubleToDate(spaceTime.SpaceEnd);
                myWork.CV_QT_MyWork.CreatedDate = DateTime.Now;
                _context.CV_QT_MyWork.Add(myWork.CV_QT_MyWork);
                // lưu quy trình luân chuyển công việc
                CV_QT_WorkFlow wflow = WorksCommon.objWorkFlow(_context, myWork.CV_QT_MyWork.Id, userId, userId, 13, "CV_MYWORK", null, myWork.CV_QT_MyWork.Note, "Công việc khởi tạo sau", 1);
                wflow.ReadDate = DateTime.Now;
                wflow.Readed = true;
                _context.CV_QT_WorkFlow.Add(wflow);
                // lưu quy trình luân chuyển công việc
                foreach (var item in myWork.CV_QT_MySupportWorks)
                {
                    item.MyWorkId = myWork.CV_QT_MyWork.Id;
                    _context.CV_QT_MySupportWork.Add(item);
                }
                CV_QT_MyScheduleWork myScheduleWork = new CV_QT_MyScheduleWork()
                {

                    TaskName = myWork.CV_QT_MyWork.TaskName,
                    FullName = user.FullName,
                    UserCreateId = userId,
                    UserDeliverId = userId,
                    StartDate = myWork.CV_QT_MyWork.ExpectedDate,
                    EndDate = myWork.CV_QT_MyWork.EndDate,
                    MyWorkId = myWork.CV_QT_MyWork.Id,
                    Predecessor = 0,
                    WorkTime = 0,
                    StatusWork = 0
                };
                _context.CV_QT_MyScheduleWork.Add(myScheduleWork);
                if (Request.Form.Files.Count != 0)
                {
                    foreach (var item in Request.Form.Files)
                    {
                        CV_QT_WorkFlowFile obj = new CV_QT_WorkFlowFile();
                        var file = item;
                        var folderName = Path.Combine("Resources", "WorkFlows", "Myworks");
                        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                        if (!Directory.Exists(pathToSave))
                        {
                            Directory.CreateDirectory(pathToSave);
                        }
                        if (myWork != null)
                        {
                            if (file.Length > 0)
                            {
                                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                                var fullPath = Path.Combine(pathToSave, fileName);
                                var dbPath = Path.Combine(folderName, fileName);
                                obj.Extension = Path.GetExtension(fileName);
                                obj.Path = dbPath;
                                using (var stream = new FileStream(fullPath, FileMode.Create))
                                {
                                    file.CopyTo(stream);
                                }
                            }
                        }
                        obj.Name = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        obj.WorkFlowId = wflow.Id;
                        obj.Size = Convert.ToDouble(file.Length / 1048576);

                        _context.CV_QT_WorkFlowFile.Add(obj);
                    }

                }
                spaceTime.Handled = true;
                spaceTime.HandledDate = DateTime.Now;
                spaceTime.HandledUserId = userId;
                spaceTime.MyWorkNewId = myWork.CV_QT_MyWork.Id;

                List<CV_QT_DepartmentSupporter> listDep = new List<CV_QT_DepartmentSupporter>();
                foreach (var item in myWork.CV_QT_DepartmentSupporter)
                {
                    CV_QT_DepartmentSupporter _QT_DepartmentSupporter = new CV_QT_DepartmentSupporter()
                    {
                        MyWorkId = myWork.CV_QT_MyWork.Id,
                        DepartmentId = item.DepartmentId,
                        DepartmentName = _context.Sys_Dm_Department.Find(item.DepartmentId) != null ? _context.Sys_Dm_Department.Find(item.DepartmentId).Name : null
                    };
                    listDep.Add(_QT_DepartmentSupporter);
                }
                _context.CV_QT_DepartmentSupporter.AddRange(listDep);
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0 });

            }
            catch (Exception e)
            {
                bool success = SaveLog.SaveLogEx(_context, "api/MyWork/r2AddDataMywork", e.Message, "Thêm mới công việc");
                return new ObjectResult(new { error = 1, ms = "Lỗi khi thêm mới công việc!" });
            }
        }
        #endregion
        #region Xóa MyWorks
        //Post: api/MyWork/r4RemoveMyWork
        [HttpPost]
        [Route("r4RemoveMyWork")]
        public async Task<ActionResult<IEnumerable<CV_QT_MyWork>>> r4RemoveMyWork(WorkFlow workFlow)
        {
            try
            {
                var myWork = await _context.CV_QT_MyWork.FindAsync(workFlow.MyWorkId);
                var workFlowDelete = _context.CV_QT_WorkFlow.Where(x => x.MyWorkId == workFlow.MyWorkId).Select(x => x.TypeFlow).Distinct().ToList();
                if (workFlowDelete.Contains(13) && workFlowDelete.Contains(6))
                {
                    bool success = SaveLog.SaveLogEx(_context, "api/MyWork/r4RemoveMyWork", "NotContent()", "Xóa mywork");
                    return new ObjectResult(new { error = 1, ms = "Không thể xóa công việc khởi tạo sau vì đã được duyệt!" });
                }
                else if (workFlowDelete.Contains(13))
                {
                    if (myWork == null)
                    {
                        bool success = SaveLog.SaveLogEx(_context, "api/MyWork/r4RemoveMyWork", "NotContent()", "Xóa mywork");
                        return new ObjectResult(new { error = 1, ms = "Lỗi khi xóa công việc của tôi!" });
                    }
                    var workFlowOlds = _context.CV_QT_WorkFlow.Where(x => x.MyWorkId == workFlow.MyWorkId);
                    if (workFlowOlds.Count() >= 2)
                    {
                        return new ObjectResult(new { error = 1, ms = "Công việc đã được thực hiện nhiều quy trình, không thể xóa!" });
                    }
                    var spaceTime = _context.CV_QT_SpaceTimeOnDay.FirstOrDefault(x => x.MyWorkNewId == workFlow.MyWorkId);
                    if (spaceTime != null)
                    {
                        spaceTime.Handled = false;
                        spaceTime.HandledUserId = null;
                        spaceTime.MyWorkNewId = null;
                    }
                    _context.CV_QT_MyWork.Remove(myWork);
                    _context.CV_QT_WorkFlow.RemoveRange(workFlowOlds);
                }
                else
                {
                    if (myWork == null)
                    {
                        bool success = SaveLog.SaveLogEx(_context, "api/MyWork/r4RemoveMyWork", "NotContent()", "Xóa mywork");
                        return new ObjectResult(new { error = 1, ms = "Lỗi khi xóa công việc của tôi!" });
                    }
                    if (myWork.CycleWork != 0)
                    {
                        return new ObjectResult(new { error = 1, ms = "Công việc đã được thực hiện, không thể xóa!" });
                    }
                    var workFlows = _context.CV_QT_WorkFlow.Where(x => x.MyWorkId == workFlow.MyWorkId);
                    if (workFlows.Count() >= 2)
                    {
                        return new ObjectResult(new { error = 1, ms = "Công việc đã được thực hiện nhiều quy trình, không thể xóa!" });
                    }

                    _context.CV_QT_MyWork.Remove(myWork);
                    _context.CV_QT_WorkFlow.RemoveRange(workFlows);
                }

                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0, ms = "Xóa thành công công việc của tôi!" });

            }
            catch (Exception e)
            {
                bool success = SaveLog.SaveLogEx(_context, "api/MyWork/r4RemoveMyWork", e.Message, "Thêm kế hoạch tự lập");
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Thêm kế hoạch tự lập
        //Post: api/MyWork/r2AddScheduleMyWork
        [HttpPost]
        [Route("r2AddScheduleMyWork")]
        public async Task<ActionResult<IEnumerable<CV_QT_MyWork>>> r2AddScheduleMyWork(CV_QT_MyScheduleWork model)
        {
            try
            {
                int userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var userDeliver = await _context.Sys_Dm_User.FindAsync(model.UserDeliverId);
                model.UserCreateId = userId;
                model.WorkTime = (model.EndDate.Value - model.StartDate.Value).TotalHours;
                model.FullName = userDeliver.FullName;
                _context.CV_QT_MyScheduleWork.Add(model);
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0, data = model.MyWorkId });

            }
            catch (Exception e)
            {
                bool success = SaveLog.SaveLogEx(_context, "api/MyWork/r2AddScheduleMyWork", e.Message, "Thêm kế hoạch tự lập");
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Xóa kế hoạch tự lập
        //Post: api/MyWork/r4RemoveScheduleMyWork
        [HttpPost]
        [Route("r4RemoveScheduleMyWork")]
        public async Task<ActionResult<IEnumerable<CV_QT_MyScheduleWork>>> r4RemoveScheduleMyWork(CV_QT_MyScheduleWork model)
        {
            try
            {
                var schedule = await _context.CV_QT_MyScheduleWork.FindAsync(model.Id);
                if (schedule == null)
                {
                    bool success = SaveLog.SaveLogEx(_context, "api/MyWork/r4RemoveScheduleMyWork", "NotContent()", "Thêm kế hoạch tự lập");
                    return new ObjectResult(new { error = 1, ms = "Lỗi khi xóa kế hoạch công việc!" });
                }
                _context.CV_QT_MyScheduleWork.Remove(schedule);
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0, ms = "Xóa thành công kế hoạch công việc!" });

            }
            catch (Exception e)
            {
                bool success = SaveLog.SaveLogEx(_context, "api/MyWork/r4RemoveScheduleMyWork", e.Message, "Thêm kế hoạch tự lập");
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Danh sách công việc MyWork
        //Post: api/MyWork/r1GetListMyWorks
        [HttpGet]
        [Route("r1GetListMyWorks")]
        public async Task<ActionResult<IEnumerator<CV_QT_MyWork>>> r1GetListMyWorks()
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                // tam thoi su dung 2 danh sách để thực hiện where trong html sau này sẽ chỉnh lại trong backend
                // Danh sách typeflow lấy ra tạm thời trong khi tìm cách xử lý trong backend
                // Sẽ lấy 1 danh sách thay vì phải sử dụng 2 danh sách rồi xử lý phía client
                var TypeFlows = (from n in _context.CV_QT_WorkFlow
                                 join v in _context.CV_QT_MyWork on n.MyWorkId equals v.Id
                                 where v.UserTaskId == userId && n.TypeFlow != 0 && n.TypeFlow != 11 && n.TypeFlow != 12
                                 group n by new { n.TypeFlow, n.MyWorkId } into g
                                 select new
                                 {
                                     g.Key.TypeFlow,
                                     g.Key.MyWorkId,
                                 }).ToList();
                var myWorks = (from a in _context.CV_QT_MyWork
                               join b in _context.CV_QT_WorkFlow on a.Id equals b.MyWorkId
                               where a.UserTaskId == userId && b.UserSendId == b.UserDeliverId && b.TypeFlow == 0
                               orderby b.CreateDate descending
                               select new
                               {
                                   MyWorkId = a.Id,
                                   b.Id,
                                   a.Note,
                                   a.Code,
                                   TaskName = a.TaskCode == null ? a.TaskName : "(" + a.TaskCode + ") " + a.TaskName,
                                   a.TaskId,
                                   a.Predecessor,
                                   a.StartDate,
                                   a.EndDate,
                                   Status = WorksCommon.getTrangThaiKetThucCv(a.TypeComplete ?? 0, a.EndDate ?? DateTime.Now, a.CompleteDate ?? DateTime.Now),
                                   a.CycleWork,
                                   a.DeliverType,
                                   a.DepartmentId,
                                   a.PauseTime,
                                   a.WorkTime,
                                   a.UserTaskName,
                                   a.TypeComplete,
                                   a.CompleteDate,
                                   a.ExpectedDate
                               });
                var qrs = await WorksCommon.Paginate(myWorks, 0, 1000).ToListAsync();

                return new ObjectResult(new { error = 0, data = qrs, TypeFlows, total = myWorks.Count() });
            }
            catch (Exception e)
            {
                bool success = SaveLog.SaveLogEx(_context, "api/MyWork/r1GetListMyWorks", e.Message, "Danh sách công việc myworks");
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Danh sách công việc MyWork khởi tạo sau
        //Post: api/MyWork/r1GetListMyWorksOld
        [HttpGet]
        [Route("r1GetListMyWorksOld")]
        public async Task<ActionResult<IEnumerator<CV_QT_MyWork>>> r1GetListMyWorksOld()
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                // tam thoi su dung 2 danh sách để thực hiện where trong html sau này sẽ chỉnh lại trong backend
                // Danh sách typeflow lấy ra tạm thời trong khi tìm cách xử lý trong backend
                // Sẽ lấy 1 danh sách thay vì phải sử dụng 2 danh sách rồi xử lý phía client
                var TypeFlows = (from n in _context.CV_QT_WorkFlow
                                 join v in _context.CV_QT_MyWork on n.MyWorkId equals v.Id
                                 where v.UserTaskId == userId && n.TypeFlow != 0 && n.TypeFlow != 11 && n.TypeFlow != 12
                                 group n by new { n.TypeFlow, n.MyWorkId } into g
                                 select new
                                 {
                                     g.Key.TypeFlow,
                                     g.Key.MyWorkId,
                                 }).ToList();
                var myWorks = (from a in _context.CV_QT_MyWork
                               join b in _context.CV_QT_WorkFlow on a.Id equals b.MyWorkId
                               where a.UserTaskId == userId && b.UserSendId == b.UserDeliverId && b.TypeFlow == 13
                               orderby b.CreateDate descending
                               select new
                               {
                                   MyWorkId = a.Id,
                                   b.Id,
                                   a.Note,
                                   a.Code,
                                   TaskName = a.TaskCode == null ? a.TaskName : "(" + a.TaskCode + ") " + a.TaskName,
                                   a.TaskId,
                                   a.Predecessor,
                                   a.StartDate,
                                   a.EndDate,
                                   Status = WorksCommon.getTrangThaiKetThucCv(a.TypeComplete ?? 0, a.EndDate ?? DateTime.Now, a.CompleteDate ?? DateTime.Now, b.TypeFlow),
                                   a.CycleWork,
                                   a.DeliverType,
                                   a.DepartmentId,
                                   a.PauseTime,
                                   a.WorkTime,
                                   a.UserTaskName,
                                   a.TypeComplete,
                                   a.CompleteDate,
                                   a.ExpectedDate,
                                   b.TypeFlow
                               });
                var qrs = await WorksCommon.Paginate(myWorks, 0, 1000).ToListAsync();

                return new ObjectResult(new { error = 0, data = qrs, TypeFlows, total = myWorks.Count() });
            }
            catch (Exception e)
            {
                bool success = SaveLog.SaveLogEx(_context, "api/MyWork/r1GetListMyWorks", e.Message, "Danh sách công việc myworks");
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Danh sách công việc MyWork tôi phối hợp
        //Post: api/MyWork/r1GetListMyWorksMySupport
        [HttpGet]
        [Route("r1GetListMyWorksMySupport")]
        public async Task<ActionResult<IEnumerator<CV_QT_MyWork>>> r1GetListMyWorksMySupport()
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var myWorkIdMySuport = await _context.CV_QT_MySupportWork.Where(x => x.UserId == userId).Select(a => a.MyWorkId).ToListAsync();
                // tam thoi su dung 2 danh sách để thực hiện where trong html sau này sẽ chỉnh lại trong backend
                // Danh sách typeflow lấy ra tạm thời trong khi tìm cách xử lý trong backend
                // Sẽ lấy 1 danh sách thay vì phải sử dụng 2 danh sách rồi xử lý phía client
                var TypeFlows = (from n in _context.CV_QT_WorkFlow
                                 join v in _context.CV_QT_MyWork on n.MyWorkId equals v.Id
                                 where v.UserTaskId == userId && n.TypeFlow != 0 && n.TypeFlow != 11 && n.TypeFlow != 12
                                 group n by new { n.TypeFlow, n.MyWorkId } into g
                                 select new
                                 {
                                     g.Key.TypeFlow,
                                     g.Key.MyWorkId,
                                 }).ToList();
                var myWorks = (from a in _context.CV_QT_MyWork
                               join b in _context.CV_QT_WorkFlow on a.Id equals b.MyWorkId
                               where myWorkIdMySuport.Contains(a.Id)
                               orderby b.CreateDate descending
                               select new
                               {
                                   MyWorkId = a.Id,
                                   b.Id,
                                   a.Note,
                                   a.Code,
                                   TaskName = a.TaskCode == null ? a.TaskName : "(" + a.TaskCode + ") " + a.TaskName,
                                   a.TaskId,
                                   a.Predecessor,
                                   a.StartDate,
                                   a.EndDate,
                                   Status = WorksCommon.getTrangThaiKetThucCv(a.TypeComplete ?? 0, a.EndDate ?? DateTime.Now, a.CompleteDate ?? DateTime.Now),
                                   a.CycleWork,
                                   a.DeliverType,
                                   a.DepartmentId,
                                   a.PauseTime,
                                   a.WorkTime,
                                   a.UserTaskName,
                                   a.TypeComplete,
                                   a.CompleteDate,
                                   a.ExpectedDate
                               });
                var qrs = await WorksCommon.Paginate(myWorks, 0, 1000).ToListAsync();

                return new ObjectResult(new { error = 0, data = qrs, TypeFlows, total = myWorks.Count() });
            }
            catch (Exception e)
            {
                bool success = SaveLog.SaveLogEx(_context, "api/MyWork/r1GetListMyWorks", e.Message, "Danh sách công việc myworks");
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Danh sách công việc có thời gian làm ngoài giờ
        //Post: api/MyWork/r1GetListMyWorkOverTimes
        [HttpGet]
        [Route("r1GetListMyWorkOverTimes")]
        public async Task<ActionResult<IEnumerator<CV_QT_MyWork>>> r1GetListMyWorkOverTimes()
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var user = await _context.Sys_Dm_User.FindAsync(userId);
                var users = _context.Sys_Dm_User.Where(x => x.ParentDepartId == user.ParentDepartId).Select(c => c.Id);
                var TypeFlows = (from n in _context.CV_QT_WorkFlow
                                 join v in _context.CV_QT_MyWork on n.MyWorkId equals v.Id
                                 where v.UserTaskId == userId && n.TypeFlow != 0 && n.TypeFlow != 11 && n.TypeFlow != 12
                                 group n by new { n.TypeFlow, n.MyWorkId } into g
                                 select new
                                 {
                                     g.Key.TypeFlow,
                                     g.Key.MyWorkId,
                                 }).ToList();
                var myWorks = (from a in _context.CV_QT_MyWork
                               join b in _context.CV_QT_WorkFlow on a.Id equals b.MyWorkId
                               where _context.CV_QT_WorkNote.Count(x => x.MyWorkId == a.Id && x.OverTime == true && x.Handle != true && x.WorkTime >= 0.5) > 0
                               && users.Contains(a.UserTaskId) && b.TypeFlow == 0
                               orderby b.CreateDate descending
                               select new
                               {
                                   MyWorkId = a.Id,
                                   b.Id,
                                   a.Note,
                                   a.Code,
                                   TaskName = a.TaskCode == null ? a.TaskName : "(" + a.TaskCode + ") " + a.TaskName,
                                   a.TaskId,
                                   a.Predecessor,
                                   a.StartDate,
                                   a.EndDate,
                                   Status = WorksCommon.getTrangThaiKetThucCv(a.TypeComplete ?? 0, a.EndDate ?? DateTime.Now, a.CompleteDate ?? DateTime.Now),
                                   a.CycleWork,
                                   a.DeliverType,
                                   a.DepartmentId,
                                   a.PauseTime,
                                   a.WorkTime,
                                   a.UserTaskName,
                                   a.TypeComplete,
                                   a.CompleteDate,
                                   a.ExpectedDate
                               });
                var qrs = await WorksCommon.Paginate(myWorks, 0, 1000).ToListAsync();

                return new ObjectResult(new { error = 0, data = qrs, TypeFlows, total = myWorks.Count() });
            }
            catch (Exception e)
            {
                bool success = SaveLog.SaveLogEx(_context, "api/MyWork/r1GetListMyWorkOverTimes", e.Message, "Danh sách công việc myworks overtime");
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Lấy 1 công việc theo MyworkId và FlowWorkId
        //Post: api/MyWork/r1GetMyWorkById
        [HttpPost]
        [Route("r1GetMyWorkById")]
        public async Task<ActionResult<IEnumerator<CV_QT_MyWork>>> r1GetMyWorkById(WorkFlow workFlow)
        {
            try
            {

                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);

                var query = from a in _context.CV_QT_MySupportWork.ToList()
                            where a.MyWorkId == workFlow.MyWorkId
                            group a by a.MyWorkId into g
                            select new
                            {
                                EmployeeId = g.Key,
                                SpecialtyCode = string.Join(", ", g.Select(x =>
                                x.FullName))
                            };
                var queryDep = from a in _context.CV_QT_DepartmentSupporter.ToList()
                            where a.MyWorkId == workFlow.MyWorkId
                            group a by a.MyWorkId into g
                            select new
                            {
                                Id = g.Key,
                                DepName = string.Join(", ", g.Select(x =>
                                x.DepartmentName))
                            };
                var historyIq = (from a in _context.CV_QT_StartPauseHistory
                                 join b in _context.CV_QT_MyWork on a.MyWorkId equals b.Id
                                 where a.MyWorkId == workFlow.MyWorkId
                                 select new
                                 {
                                     b.TaskCode,
                                     b.TaskName,
                                     a.CreateDate,
                                     a.CycleWork,
                                     a.Id,
                                 }).AsQueryable();
                var history = await historyIq.OrderByDescending(x => x.CreateDate).ToListAsync();
                DateTime timeWorkStart = DateTime.Now;
                if (historyIq.FirstOrDefault(x => x.CycleWork == 1) != null)
                {
                    timeWorkStart = historyIq.FirstOrDefault(x => x.CycleWork == 1).CreateDate; // lay ra thoi gian bat dau thuc hien cong viec
                }
                var myWork = await (from a in _context.CV_QT_MyWork
                                    where a.Id == workFlow.MyWorkId
                                    select new
                                    {
                                        a.Code,
                                        a.TaskCode,
                                        a.TaskName,
                                        WorkTime = WorksCommon.setTimeWorkStep(timeWorkStart, a.CycleWork, a.WorkTime ?? 0.0, a.EndPause),
                                        a.CycleWork,
                                        a.PauseTime,
                                        a.EndDate,
                                        a.StartDate,
                                        a.UserTaskName,
                                        a.Note,
                                        a.PreWorkDeadline,
                                        Suporter = query.FirstOrDefault() != null ? query.FirstOrDefault().SpecialtyCode : null,
                                        DepSuporter = queryDep.FirstOrDefault() != null ? queryDep.FirstOrDefault().DepName : null,
                                        TotalPoint = _context.CV_QT_CounterError.Count(x => x.MyWorkId == workFlow.MyWorkId),
                                        PreWorkType = a.PreWorkType == true ? "Hoàn thành cv tiên quyết" : "Không",
                                        a.Predecessor,
                                        a.ExpectedDate,
                                        a.CompleteDate
                                    }).FirstOrDefaultAsync();
                // danh sach tong hop thoi gian lam ngoai h cua 1 cong viec
                var workOvertime = await _context.CV_QT_WorkNote.Where(x => x.MyWorkId == workFlow.MyWorkId && x.DateEnd != null && x.OverTime == true).GroupBy(x => new
                {
                    x.DateEnd.Value.Date,
                    x.MyWorkId,
                    x.Handle,
                    x.State
                }, x => x.WorkTime).Select(a => new
                {
                    a.Key.Handle,
                    a.Key.MyWorkId,
                    a.Key.State,
                    DateOverTime = TransforDate.FromDateToDouble(a.Key.Date),
                    WorkTime = a.Sum()
                }).ToListAsync();

                // cong viec truoc
                var predecWork = await (from a in _context.CV_QT_MyWork
                                        where a.Code == myWork.Predecessor
                                        select new
                                        {
                                            a.Code,
                                            a.TaskCode,
                                            a.TaskName,
                                            WorkTime = WorksCommon.setTimeWorkStep(timeWorkStart, a.CycleWork, a.WorkTime ?? 0.0, a.EndPause),
                                            a.CycleWork,
                                            a.PauseTime,
                                            a.EndDate,
                                            a.StartDate,
                                            a.UserTaskName,
                                            a.Note,
                                            Suporter = query.FirstOrDefault() != null ? query.FirstOrDefault().SpecialtyCode : null,
                                            TotalPoint = _context.CV_QT_CounterError.Count(x => x.MyWorkId == workFlow.MyWorkId),
                                            a.Predecessor,
                                            a.ExpectedDate,
                                            a.CompleteDate
                                        }).FirstOrDefaultAsync();
                var errors = await (from a in _context.CV_QT_CounterError
                                    join b in _context.CV_DM_Error on a.ErrorId equals b.Id
                                    join c in _context.Sys_Dm_User on a.NguoiPhatId equals c.Id
                                    join d in _context.Sys_Dm_User on a.NguoiBiPhatId equals d.Id
                                    where a.MyWorkId == workFlow.MyWorkId
                                    select new
                                    {
                                        a.Point,
                                        NguoiPhat = c.FullName,
                                        b.ErrorName,
                                        a.CreateDate,
                                        NguoiBiPhat = d.FullName
                                    }).ToListAsync();
                var workFlows = await (from a in _context.CV_QT_WorkFlow
                                       join b in _context.CV_QT_MyWork on a.MyWorkId equals b.Id
                                       where a.MyWorkId == workFlow.MyWorkId
                                       orderby a.SendDate descending
                                       select new
                                       {
                                           a.Id,
                                           a.DeliverName,
                                           a.SendName,
                                           a.SendDate,
                                           a.PositionSend,
                                           a.DepartDeli,
                                           a.PositionDeli,
                                           a.Note,
                                           a.Require,
                                           Files = _context.CV_QT_WorkFlowFile.Where(x => x.WorkFlowId == a.Id).Select(v => new
                                           {
                                               v.Path,
                                               v.Name,
                                               v.Size
                                           }).ToList(),
                                           a.Repossibility,
                                           a.ReadDate,
                                           a.HandleDate,
                                           a.TypeFlow,
                                           a.ParentId
                                       }).ToListAsync();
                // lấy ra dữ liệu workflow công việc tiên quyết
                var workFlowPres = await (from a in _context.CV_QT_WorkFlow
                                          join b in _context.CV_QT_MyWork on a.MyWorkId equals b.Id
                                          where b.Code == myWork.Predecessor
                                          orderby a.SendDate descending
                                          select new
                                          {
                                              a.Id,
                                              a.DeliverName,
                                              a.SendName,
                                              a.SendDate,
                                              a.PositionSend,
                                              a.DepartDeli,
                                              a.PositionDeli,
                                              a.Note,
                                              a.Require,
                                              Files = _context.CV_QT_WorkFlowFile.Where(x => x.WorkFlowId == a.Id).Select(v => new
                                              {
                                                  v.Path,
                                                  v.Name,
                                                  v.Size
                                              }).ToList(),
                                              a.Repossibility,
                                              a.ReadDate,
                                              a.HandleDate,
                                              a.TypeFlow,
                                              a.ParentId
                                          }).ToListAsync();
                var supports = await (from a in _context.CV_QT_MySupportWork
                                      where a.MyWorkId == workFlow.MyWorkId
                                      select new
                                      {
                                          a.UserId,
                                          a.FullName,
                                          a.Id,
                                      }).ToListAsync();
                var files = await _context.CV_QT_WorkFlowFile.Where(x => x.WorkFlowId == workFlow.Id).ToListAsync();
                var cV_QT_WorkFlows = _context.CV_QT_WorkFlow.Where(x => x.UserDeliverId == userId && x.MyWorkId == workFlow.MyWorkId);
                foreach (var item in cV_QT_WorkFlows)
                {
                    var cV_QT_WorkFlow = await _context.CV_QT_WorkFlow.FindAsync(item.Id);
                    if (cV_QT_WorkFlow.Readed != true)
                    {
                        cV_QT_WorkFlow.ReadDate = DateTime.Now;
                        cV_QT_WorkFlow.Readed = true;
                    }
                }
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0, data = myWork, predecWork, files, history, workFlows, errors, supports, workFlowPres, workOvertime });
            }
            catch (Exception e)
            {
                bool success = SaveLog.SaveLogEx(_context, "api/MyWork/r1GetMyWorkById", e.Message, "Lấy 1 công việc theo MyworkId và FlowWorkId");
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Trạng thái duyệt theo Mã công việc
        // Post: api/MyWork/r1GetStateSignOfMyWork
        [HttpPost]
        [Route("r1GetStateSignOfMyWork")]
        public async Task<ActionResult> r1GetStateSignOfMyWork(WorkFlow workFlow)
        {
            try
            {
                int[] duyetFlow = {2, 3, 5, 6, 15, 16 };
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var typeFlows = _context.CV_QT_WorkFlow.Where(x => x.MyWorkId == workFlow.MyWorkId && duyetFlow.Contains(x.TypeFlow) == true).Select(x => x.TypeFlow);
                return new ObjectResult(new { error = 0, data = typeFlows });
            }
            catch (Exception e)
            {
                bool success = SaveLog.SaveLogEx(_context, "api/MyWork/r1GetStateSignOfMyWork", e.Message, "Mã công việc");
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Danh sách công việc overtime
        // Post: api/MyWork/r1PostMyWorkOverTime
        [HttpPost]
        [Route("r1PostMyWorkOverTime")]
        public async Task<ActionResult<IEnumerable<CV_QT_MyScheduleWork>>> r1PostMyWorkOverTime(WorkFlow workFlow)
        {
            try
            {

                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                //var myWorkOverTimes = _context.CV_QT_WorkNote
                //    .Where(x => x.Handle != true && x.OverTime == true && x.MyWorkId == workFlow.MyWorkId && x.DateEnd != null)
                //    .GroupBy(x => x.MyWorkId)
                //    .Select(a => new
                //    {
                //        a.FirstOrDefault().Date,
                //        a.Sum(x=>x.)
                //    });
                return new ObjectResult(new { error = 0 });

            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Danh sách khoảng trống thời gian
        // Get: api/MyWork/r1PostMyWorkSpaceTime
        [HttpGet]
        [Route("r1GetMyWorkSpaceTime")]
        public async Task<ActionResult<IEnumerable<CV_QT_MyScheduleWork>>> r1GetMyWorkSpaceTime()
        {
            try
            {

                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var myWorkSpaceTimes = await (from a in _context.CV_QT_SpaceTimeOnDay
                                              join b in _context.CV_QT_MyWork on a.MyWorkId equals b.Id
                                              where a.Handled != true && a.UserId == userId && a.Time >= 60
                                              select new
                                              {
                                                  a.Id,
                                                  a.MyWorkId,
                                                  Name = "(Mã công việc: " + b.Code + ") " + b.TaskCode + " " + b.TaskName,
                                                  SpaceStart = TransforDate.FromDoubleToDate(a.SpaceStart),
                                                  SpaceEnd = TransforDate.FromDoubleToDate(a.SpaceEnd),
                                                  a.Time
                                              }).ToListAsync();

                return new ObjectResult(new { error = 0, data = myWorkSpaceTimes });

            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Trình phê duyệt thời gian
        //Post: api/MyWork/r2AddFLowDeadline
        [HttpPost, DisableRequestSizeLimit]
        [Route("r2AddFLowDeadline")]
        public async Task<ActionResult<IEnumerable<CV_QT_MyWork>>> r2AddFLowDeadline()
        {
            try
            {
                var myWork = JsonConvert.DeserializeObject<CV_QT_WorkFlow>(Request.Form["model"]);
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                // lưu quy trình luân chuyển công việc
                CV_QT_WorkFlow wflow = WorksCommon.objWorkFlow(_context, myWork.MyWorkId, userId, myWork.UserDeliverId, 1, "CV_MYWORK", myWork.ParentId, myWork.Note, myWork.Require, 1);
                _context.CV_QT_WorkFlow.Add(wflow);
                if (Request.Form.Files.Count != 0)
                {
                    foreach (var item in Request.Form.Files)
                    {
                        CV_QT_WorkFlowFile obj = new CV_QT_WorkFlowFile();
                        var file = item;
                        var folderName = Path.Combine("Resources", "WorkFlows", "Deadlines");
                        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                        if (!Directory.Exists(pathToSave))
                        {
                            Directory.CreateDirectory(pathToSave);
                        }
                        if (myWork != null)
                        {
                            if (file.Length > 0)
                            {
                                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                                var fullPath = Path.Combine(pathToSave, fileName);
                                var dbPath = Path.Combine(folderName, fileName);
                                obj.Extension = Path.GetExtension(fileName);
                                obj.Path = dbPath;
                                using (var stream = new FileStream(fullPath, FileMode.Create))
                                {
                                    file.CopyTo(stream);
                                }
                            }
                        }
                        obj.Name = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        obj.WorkFlowId = wflow.Id;
                        obj.Size = file.Length / 1048576;

                        _context.CV_QT_WorkFlowFile.Add(obj);
                    }

                }
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0 });

            }
            catch (Exception e)
            {
                bool success = SaveLog.SaveLogEx(_context, "api/MyWork/r1GetListMyWorks", e.Message, "Trình phê duyệt thời hạn");
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Trình phê duyệt hoàn thành
        //Post: api/MyWork/r2AddFLowCheckSuccess
        [HttpPost, DisableRequestSizeLimit]
        [Route("r2AddFLowCheckSuccessful")]
        public async Task<ActionResult<IEnumerable<CV_QT_MyWork>>> r2AddFLowCheckSuccessful()
        {
            try
            {
                var model = JsonConvert.DeserializeObject<Dtos_FlowWork>(Request.Form["model"]);
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                // lưu quy trình luân chuyển công việc
                CV_QT_WorkFlow wflow = WorksCommon.objWorkFlow(_context, model.CV_QT_WorkFlow.MyWorkId, userId, model.CV_QT_WorkFlow.UserDeliverId, 4, "CV_MYWORK", model.CV_QT_WorkFlow.ParentId, model.CV_QT_WorkFlow.Note, model.CV_QT_WorkFlow.Require, 1);
                _context.CV_QT_WorkFlow.Add(wflow);
                List<CV_QT_WorkFlowFile> _WorkFlowFiles = new List<CV_QT_WorkFlowFile>();
                if (Request.Form.Files.Count != 0)
                {
                    foreach (var item in Request.Form.Files)
                    {
                        CV_QT_WorkFlowFile obj1 = new CV_QT_WorkFlowFile();
                        var file = item;
                        var folderName = Path.Combine("Resources", "WorkFlows", "successful");
                        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                        if (!Directory.Exists(pathToSave))
                        {
                            Directory.CreateDirectory(pathToSave);
                        }
                        if (model != null)
                        {
                            if (file.Length > 0)
                            {
                                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                                var fullPath = Path.Combine(pathToSave, fileName);
                                var dbPath = Path.Combine(folderName, fileName);
                                obj1.Extension = Path.GetExtension(fileName);
                                obj1.Path = dbPath;
                                using (var stream = new FileStream(fullPath, FileMode.Create))
                                {
                                    file.CopyTo(stream);
                                }
                            }
                        }
                        obj1.Name = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        obj1.WorkFlowId = wflow.Id;
                        obj1.Size = file.Length / 1048576;
                        _WorkFlowFiles.Add(obj1);
                        _context.CV_QT_WorkFlowFile.Add(obj1);
                    }

                }
                // lưu quy trình người xác nhận hoàn thành người nhận công việc tiếp theo
                if (model.CV_QT_NextPlan.UserId != 0)
                {
                    CV_QT_WorkFlow wnext = new CV_QT_WorkFlow();
                    wnext = WorksCommon.objWorkFlow(_context, model.CV_QT_WorkFlow.MyWorkId, userId, model.CV_QT_NextPlan.UserId, 4, "CV_MYWORK", model.CV_QT_WorkFlow.ParentId, model.CV_QT_WorkFlow.Note, model.CV_QT_WorkFlow.Require, 2);
                    _context.CV_QT_WorkFlow.Add(wnext);
                    if (_WorkFlowFiles.Count != 0)
                    {
                        foreach (var item in _WorkFlowFiles)
                        {
                            CV_QT_WorkFlowFile obj2 = new CV_QT_WorkFlowFile();
                            obj2.Extension = item.Extension;
                            obj2.Path = item.Path;
                            obj2.Name = item.Name;
                            obj2.WorkFlowId = wnext.Id;
                            obj2.Size = item.Size;

                            _context.CV_QT_WorkFlowFile.Add(obj2);
                        }

                    }
                }
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0 });

            }
            catch (Exception e)
            {
                bool success = SaveLog.SaveLogEx(_context, "api/MyWork/r2AddFLowCheckSuccess", e.Message, "Trình phê duyệt hoàn thành");
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Trình phê duyệt xin thêm thời hạn
        //Post: api/MyWork/r2AddXinThemThoiHan
        [HttpPost]
        [Route("r2AddXinThemThoiHan")]
        public async Task<ActionResult<IEnumerable<CV_QT_MyWork>>> r2AddXinThemThoiHan()
        {
            try
            {
                var model = JsonConvert.DeserializeObject<Dtos_FlowWorkPheDuyetTH>(Request.Form["model"]);
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                // lưu quy trình luân chuyển công việc
                CV_QT_WorkFlow wflow = WorksCommon.objWorkFlow(_context, model.CV_QT_WorkFlow.MyWorkId, userId, model.CV_QT_WorkFlow.UserDeliverId, 7, "CV_MYWORK", model.CV_QT_WorkFlow.ParentId, model.CV_QT_WorkFlow.Note, model.CV_QT_WorkFlow.Require, 1);
                _context.CV_QT_WorkFlow.Add(wflow);

                if (Request.Form.Files.Count != 0)
                {
                    foreach (var item in Request.Form.Files)
                    {
                        CV_QT_WorkFlowFile obj1 = new CV_QT_WorkFlowFile();
                        var file = item;
                        var folderName = Path.Combine("Resources", "WorkFlows", "Deadlines");
                        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                        if (!Directory.Exists(pathToSave))
                        {
                            Directory.CreateDirectory(pathToSave);
                        }
                        if (model != null)
                        {
                            if (file.Length > 0)
                            {
                                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                                var fullPath = Path.Combine(pathToSave, fileName);
                                var dbPath = Path.Combine(folderName, fileName);
                                obj1.Extension = Path.GetExtension(fileName);
                                obj1.Path = dbPath;
                                using (var stream = new FileStream(fullPath, FileMode.Create))
                                {
                                    file.CopyTo(stream);
                                }
                            }
                        }
                        obj1.Name = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        obj1.WorkFlowId = wflow.Id;
                        obj1.Size = file.Length / 1048576;
                        _context.CV_QT_WorkFlowFile.Add(obj1);
                    }

                }
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0 });

            }
            catch (Exception e)
            {
                bool success = SaveLog.SaveLogEx(_context, "api/MyWork/r2AddXinThemThoiHan", e.Message, "Trình phê duyệt xin thêm thời hạn");
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Bắt đầu thực hiện công việc
        //Post: api/MyWork/r2StartMyWork
        [HttpPost]
        [Route("r2StartMyWork")]
        public async Task<ActionResult<IEnumerator<CV_QT_MyWork>>> r1GetStartMyWork(CV_QT_MyWork model)
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var myWork = await _context.CV_QT_MyWork.FindAsync(model.Id);
                var myWorkCount = _context.CV_QT_MyWork.Count(x => x.UserTaskId == userId && (x.CycleWork == 1 || x.CycleWork == 3));
                var myWorkPredecCount = _context.CV_QT_MyWork.Count(x => x.Code == model.Predecessor && x.TypeComplete != 3); // count coong viec tien quyet hoan thanh
                if (myWork == null)
                {
                    return NotFound();
                }
                if (myWorkCount > 0)
                {
                    return new ObjectResult(new { error = 1, ms = "Không thể bắt đầu vì có công việc khác đang diễn ra!" });
                }
                if (myWorkPredecCount > 0)
                {
                    return new ObjectResult(new { error = 1, ms = "Không thể bắt đầu vì công việc tiên quyết chưa được hoàn thành !" });
                }
                if (myWork.CycleWork == 0)
                {
                    myWork.StartDate = DateTime.Now;
                    myWork.CycleWork = 1;
                    CV_QT_StartPauseHistory his = new CV_QT_StartPauseHistory(); // lưu vào bảng lịch sử
                    his.MyWorkId = model.Id;
                    his.CreateDate = myWork.StartDate.Value;
                    his.CycleWork = 1;
                    his.UserCreateId = userId;
                    _context.CV_QT_StartPauseHistory.Add(his);
                    CV_QT_WorkNote note = new CV_QT_WorkNote(); // lưu vào bảng nhật ký công việc
                    note.MyWorkId = model.Id;
                    note.DateStart = myWork.StartDate;
                    note.WorkTime = 0.0;
                    note.CreatedBy = userId;
                    note.State = 0;
                    if (DateTime.Now.Hour >= 17) // neu bat dau sau 17 h thi tinh la overtime
                    {
                        note.OverTime = true;
                    }
                    else
                    {
                        note.OverTime = false;
                    }

                    _context.CV_QT_WorkNote.Add(note);
                    await _context.SaveChangesAsync();
                }
                return new ObjectResult(new { error = 0, ms = "Bắt đầu công việc thành công!" });
            }
            catch (Exception e)
            {
                bool success = SaveLog.SaveLogEx(_context, "api/MyWork/r2StartMyWork", e.Message, "Bắt đầu thực hiện công việc");
                return new ObjectResult(new { error = 1, ms = "Bắt đầu công việc không thành công!" });
            }
        }
        #endregion
        #region Dừng thực hiện công việc
        //Post: api/MyWork/r2PauseMyWork
        [HttpPost]
        [Route("r2PauseMyWork")]
        public async Task<ActionResult<IEnumerator<CV_QT_MyWork>>> r2PauseMyWork(CV_QT_MyWork model)
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var myWork = await _context.CV_QT_MyWork.FindAsync(model.Id);
                if (myWork == null)
                {
                    return NotFound();
                }

                if (myWork.CycleWork == 1)
                {
                    if (myWork.StartDate.Value.Hour < 17 || (myWork.StartDate.Value.Hour == 17 && myWork.StartDate.Value.Minute < 5))
                    {
                        myWork.WorkTime = myWork.WorkTime + (SpaceTimeOnDay.CalSpaceTimeOnDay(myWork.StartDate.Value, DateTime.Now) / 60);
                    }
                    myWork.StartPause = DateTime.Now;
                    myWork.CycleWork = 2;
                    CV_QT_StartPauseHistory his = new CV_QT_StartPauseHistory(); // lưu vào bảng lịch sử
                    his.MyWorkId = model.Id;
                    his.CreateDate = DateTime.Now;
                    his.CycleWork = 2;
                    his.UserCreateId = userId;
                    _context.CV_QT_StartPauseHistory.Add(his);
                    var note = await _context.CV_QT_WorkNote.FirstOrDefaultAsync(x => x.DateEnd == null && x.MyWorkId == model.Id);
                    if (note != null)
                    {
                        note.DateEnd = his.CreateDate;
                        note.WorkTime = (SpaceTimeOnDay.CalSpaceTimeOnDay(note.DateStart.Value, his.CreateDate) / 60);
                    }

                }
                else if (myWork.CycleWork == 3)
                {
                    if (myWork.EndPause.Value.Hour < 17 || (myWork.EndPause.Value.Hour == 17 && myWork.EndPause.Value.Minute < 5))
                    {
                        myWork.WorkTime = myWork.WorkTime + (SpaceTimeOnDay.CalSpaceTimeOnDay(myWork.EndPause.Value, DateTime.Now) / 60);
                    }
                    myWork.StartPause = DateTime.Now;
                    myWork.CycleWork = 2;
                    CV_QT_StartPauseHistory his = new CV_QT_StartPauseHistory(); // lưu vào bảng lịch sử
                    his.MyWorkId = model.Id;
                    his.CreateDate = DateTime.Now;
                    his.CycleWork = 2;
                    his.UserCreateId = userId;
                    _context.CV_QT_StartPauseHistory.Add(his);
                    var note = await _context.CV_QT_WorkNote.FirstOrDefaultAsync(x => x.DateEnd == null && x.MyWorkId == model.Id);
                    if (note != null)
                    {
                        note.DateEnd = his.CreateDate;
                        note.WorkTime = (SpaceTimeOnDay.CalSpaceTimeOnDay(note.DateStart.Value, his.CreateDate) / 60);
                        _context.Update(note);
                    }
                }
                else if (myWork.CycleWork == 2)
                {
                    return new ObjectResult(new { error = 2, ms = "Công việc này đã được tạm dừng trước đó!" });
                }

                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0, ms = "Tạm dừng công việc thành công!" });
            }
            catch (Exception e)
            {
                bool success = SaveLog.SaveLogEx(_context, "api/MyWork/r2PauseMyWork", e.Message, "Dừng thực hiện công việc");
                return new ObjectResult(new { error = 1, ms = "Tạm dừng công việc thành công!" });
            }
        }
        #endregion
        #region Dừng thực hiện tất cả công việc
        //Post: api/MyWork/r2PauseMyWorkAutoPause
        // được gọi từ service worker
        [HttpGet]
        [Route("r2PauseMyWorkAutoPause")]
        public async Task<ActionResult<IEnumerator<CV_QT_MyWork>>> r2PauseMyWorkAllauto()
        {
            try
            {

                var myWorks = await _context.CV_QT_MyWork.Where(x => x.CycleWork == 1 || x.CycleWork == 3).ToListAsync();
                foreach (var myWork in myWorks)
                {
                    myWork.StartPause = DateTime.Now;
                    if (myWork.CycleWork == 1)
                    {
                        myWork.WorkTime = myWork.WorkTime + (SpaceTimeOnDay.CalSpaceTimeOnDay(myWork.StartDate.Value, DateTime.Now) / 60);
                        myWork.CycleWork = 2;
                        CV_QT_StartPauseHistory his = new CV_QT_StartPauseHistory(); // lưu vào bảng lịch sử
                        his.MyWorkId = myWork.Id;
                        his.CreateDate = DateTime.Now;
                        his.CycleWork = 2;
                        his.UserCreateId = 2028;
                        _context.CV_QT_StartPauseHistory.Add(his);
                        var note = await _context.CV_QT_WorkNote.FirstOrDefaultAsync(x => x.DateEnd == null && x.MyWorkId == myWork.Id);
                        if (note != null)
                        {
                            note.DateEnd = his.CreateDate;
                            note.WorkTime = (his.CreateDate - note.DateStart.Value).TotalHours;
                        }

                    }
                    else if (myWork.CycleWork == 3)
                    {
                        myWork.WorkTime = myWork.WorkTime + (SpaceTimeOnDay.CalSpaceTimeOnDay(myWork.EndPause.Value, DateTime.Now) / 60);
                        myWork.CycleWork = 2;
                        CV_QT_StartPauseHistory his = new CV_QT_StartPauseHistory(); // lưu vào bảng lịch sử
                        his.MyWorkId = myWork.Id;
                        his.CreateDate = DateTime.Now;
                        his.CycleWork = 2;
                        his.UserCreateId = 2028;
                        _context.CV_QT_StartPauseHistory.Add(his);
                        var note = await _context.CV_QT_WorkNote.FirstOrDefaultAsync(x => x.DateEnd == null && x.MyWorkId == myWork.Id);
                        if (note != null)
                        {
                            note.DateEnd = his.CreateDate;
                            note.WorkTime = (his.CreateDate - note.DateStart.Value).TotalHours;
                        }
                    }
                }
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0 });
            }
            catch (Exception e)
            {
                bool success = SaveLog.SaveLogEx(_context, "api/MyWork/r2PauseMyWork", e.Message, "Dừng thực hiện công việc service");
                return new ObjectResult(new { error = 1, ms = "Tạm dừng công việc thành công!" });
            }
        }
        #endregion
        #region Tiếp tục thực hiện công việc
        //Post: api/MyWork/r2ContineuMyWork
        [HttpPost]
        [Route("r2ContineuMyWork")]
        public async Task<ActionResult<IEnumerator<CV_QT_MyWork>>> r2ContineuMyWork(CV_QT_MyWork model)
        {
            try
            {

                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var myWork = await _context.CV_QT_MyWork.FindAsync(model.Id);
                var myWorkCount = _context.CV_QT_MyWork.Count(x => x.UserTaskId == userId && (x.CycleWork == 1 || x.CycleWork == 3));
                if (myWork == null)
                {
                    return NotFound();
                }
                if (myWorkCount > 0)
                {
                    return new ObjectResult(new { error = 1, ms = "Không thể tiếp tục vì có công việc khác đang diễn ra!" });
                }
                if (myWork.CycleWork == 2)
                {
                    myWork.EndPause = DateTime.Now;
                    myWork.PauseTime = myWork.PauseTime + (DateTime.Now - myWork.StartPause.Value).TotalHours;
                    myWork.CycleWork = 3;
                    CV_QT_StartPauseHistory his = new CV_QT_StartPauseHistory(); // lưu vào bảng lịch sử
                    his.MyWorkId = model.Id;
                    his.CreateDate = DateTime.Now;
                    his.CycleWork = 3;
                    his.UserCreateId = userId;
                    _context.CV_QT_StartPauseHistory.Add(his);
                    CV_QT_WorkNote note = new CV_QT_WorkNote(); // lưu vào bảng nhật ký công việc
                    note.MyWorkId = model.Id;
                    note.DateStart = his.CreateDate;
                    note.WorkTime = 0.0;
                    note.CreatedBy = userId;
                    note.State = 0;
                    if (DateTime.Now.Hour >= 17)
                    {
                        note.OverTime = true;
                    }
                    else
                    {
                        note.OverTime = false;
                    }

                    _context.CV_QT_WorkNote.Add(note);
                    await _context.SaveChangesAsync();
                }
                return new ObjectResult(new { error = 0, ms = "Tiếp tục công việc thành công!" });
            }
            catch (Exception e)
            {
                bool success = SaveLog.SaveLogEx(_context, "api/MyWork/r2PauseMyWork", e.Message, "Tiếp tục thực hiện công việc");
                return new ObjectResult(new { error = 1, ms = "Tiếp tục công việc không thành công!" });
            }
        }
        #endregion
        #region Tiếp tục thực hiện công việc làm ngoài giờ
        //Post: api/MyWork/r2ContineuMyWorkOverTime
        [HttpPost]
        [Route("r2MyWorkOverTime")]
        public async Task<ActionResult<IEnumerator<CV_QT_MyWork>>> r2MyWorkOverTime(CV_QT_MyWork model)
        {
            try
            {
                if (DateTime.Now.Hour < 17)
                {
                    return new ObjectResult(new { error = 1, ms = "Nút này chỉ được sử dụng sau 17h." });
                }
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var myWork = await _context.CV_QT_MyWork.FindAsync(model.Id);
                var myWorkCount = _context.CV_QT_MyWork.Count(x => x.UserTaskId == userId && (x.CycleWork == 1 || x.CycleWork == 3));
                if (myWork == null)
                {
                    return NotFound();
                }
                if (myWorkCount > 0)
                {
                    return new ObjectResult(new { error = 1, ms = "Không thể tiếp tục vì có công việc khác đang diễn ra! Vui lòng làm mới trang." });
                }
                switch (myWork.CycleWork)
                {
                    case 0:
                        myWork.StartDate = DateTime.Now;
                        myWork.CycleWork = 1;
                        CV_QT_StartPauseHistory his = new CV_QT_StartPauseHistory(); // lưu vào bảng lịch sử
                        his.MyWorkId = model.Id;
                        his.CreateDate = DateTime.Now;
                        his.CycleWork = 1;
                        his.UserCreateId = userId;
                        _context.CV_QT_StartPauseHistory.Add(his);
                        CV_QT_WorkNote note = new CV_QT_WorkNote(); // lưu vào bảng nhật ký công việc
                        note.MyWorkId = model.Id;
                        note.DateStart = his.CreateDate;
                        note.WorkTime = 0.0;
                        note.CreatedBy = userId;
                        note.OverTime = false;
                        _context.CV_QT_WorkNote.Add(note);
                        await _context.SaveChangesAsync();
                        break;
                    default:
                        break;
                }
                return new ObjectResult(new { error = 0, ms = "Tiếp tục công việc thành công!" });
            }
            catch (Exception e)
            {
                bool success = SaveLog.SaveLogEx(_context, "api/MyWork/r2PauseMyWork", e.Message, "Tiếp tục thực hiện công việc");
                return new ObjectResult(new { error = 1, ms = "Tiếp tục công việc không thành công!" });
            }
        }
        #endregion     
        #region Danh sách kế hoạch công việc
        // Post: api/MyWork/r1PostMyScheduleWork
        [HttpPost]
        [Route("r1PostMyScheduleWork")]
        public async Task<ActionResult<IEnumerable<CV_QT_MyScheduleWork>>> r1PostMyScheduleWork(WorkFlow workFlow)
        {
            try
            {

                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var deQuys = await (from a in _context.CV_QT_MyScheduleWork
                                    where a.MyWorkId == workFlow.MyWorkId
                                    select new ScheduleMyWork
                                    {
                                        Id = a.Id,
                                        FullName = a.FullName,
                                        Predecessor = a.Predecessor,
                                        TaskName = a.TaskName,
                                        UserDeliverId = a.UserDeliverId,
                                        StartDate = a.StartDate.Value,
                                        EndDate = a.EndDate,
                                        MyWorkId = a.MyWorkId,
                                        StatusWork = a.StatusWork,
                                        DateComplete = a.DateComplete
                                    }).ToListAsync();
                var listRecursives = await (from a in _context.CV_QT_MyScheduleWork
                                            where a.MyWorkId == workFlow.MyWorkId && a.Predecessor == 0
                                            select new ScheduleMyWork()
                                            {
                                                Id = a.Id,
                                                FullName = a.FullName,
                                                Predecessor = a.Predecessor,
                                                TaskName = a.TaskName,
                                                UserDeliverId = a.UserDeliverId,
                                                StartDate = a.StartDate.Value,
                                                EndDate = a.EndDate,
                                                MyWorkId = a.MyWorkId,
                                                StatusWork = a.StatusWork,
                                                DateComplete = a.DateComplete,
                                                children = GetChildren(deQuys, a.Id)
                                            }).ToListAsync();
                return new ObjectResult(new { error = 0, listRecursives });

            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Duyệt thời hạn làm thêm giờ
        //Post: api/MyWork/r2duyetOverTime
        [HttpPost]
        [Route("r2duyetOverTime")]
        public async Task<ActionResult<IEnumerator<CV_QT_MyWork>>> r2duyetOverTime(WorkFlowOverTime model)
        {
            try
            {

                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var myWork = await _context.CV_QT_MyWork.FindAsync(model.MyWorkId);
                myWork.WorkTime += model.WorkTime;
                var notes = _context.CV_QT_WorkNote.Where(x => x.DateEnd != null
                && x.State == 0 && x.MyWorkId == model.MyWorkId
                && x.Handle != true && x.OverTime == true
                && x.DateEnd.Value.Date == TransforDate.FromDoubleToDate(model.DateOverTime));
                foreach (var note in notes)
                {
                    if (note != null)
                    {
                        note.Handle = true;
                        note.HandleDate = DateTime.Now;
                        note.HandleUserId = userId;
                        note.State = 1;
                        _context.Update(note);
                    }
                }
                await _context.SaveChangesAsync();

                return new ObjectResult(new { error = 0, ms = "Duyệt thời gian làm ngoài giờ thành công!" });
            }
            catch (Exception e)
            {
                bool success = SaveLog.SaveLogEx(_context, "api/MyWork/r2duyetOverTime", e.Message, "Duyệt thời gian làm ngoài giờ");
                return new ObjectResult(new { error = 1, ms = "Tiếp tục công việc không thành công!" });
            }
        }
        #endregion
        #region Không duyệt thời hạn làm thêm giờ
        //Post: api/MyWork/r2KhongduyetOverTime
        [HttpPost]
        [Route("r2KhongduyetOverTime")]
        public async Task<ActionResult<IEnumerator<CV_QT_MyWork>>> r2KhongduyetOverTime(WorkFlowOverTime model)
        {
            try
            {

                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var note = await _context.CV_QT_WorkNote.FirstOrDefaultAsync(x => x.DateEnd != null
                && x.State == 0 && x.OverTime == true
                && x.Handle != true
                && x.MyWorkId == model.MyWorkId
                && x.DateEnd.Value.Date == TransforDate.FromDoubleToDate(model.DateOverTime));
                if (note != null)
                {
                    note.Handle = true;
                    note.HandleDate = DateTime.Now;
                    note.HandleUserId = userId;
                    note.State = 2;
                    _context.Update(note);
                }
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0, ms = "Không duyệt thời gian làm ngoài giờ thành công!" });
            }
            catch (Exception e)
            {
                bool success = SaveLog.SaveLogEx(_context, "api/MyWork/r2KhongduyetOverTime", e.Message, "Không duyệt công việc");
                return new ObjectResult(new { error = 1, ms = "Duyệt không thành công!" });
            }
        }
        #endregion
        #region Cập nhật trạng thái công việc
        //Post: api/MyWork/r2TrangThaiScheduleMyWork
        [HttpPost]
        [Route("r2TrangThaiScheduleMyWork")]
        public async Task<IActionResult> r2TrangThaiScheduleMyWork(CV_QT_MyScheduleWork model)
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var khcv = await _context.CV_QT_MyScheduleWork.FindAsync(model.Id);
                khcv.StatusWork = model.StatusWork;
                if (model.StatusWork == 3)
                {
                    khcv.DateComplete = DateTime.Now;
                }
                else
                {
                    khcv.DateComplete = null;
                }
                khcv.UserUpdateId = userId;
                khcv.UpdateDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0, ms = "" }); ;
            }
            catch (Exception e)
            {
                bool success = SaveLog.SaveLogEx(_context, "api/MyWork/r2PauseMyWork", e.Message, "Cập nhật trạng thái công việc");
                var result = new OkObjectResult(new { error = 1, ms = "Lỗi khi cập nhật trạng thái công việc!, vui lòng kiểm tra lại!" });
                return result;
            }
        }
        #endregion
        #region Đệ quy kế hoạch công việc
        public static List<ScheduleMyWork> GetChildren(List<ScheduleMyWork> comments, int Predecessor)
        {
            return comments
                    .Where(c => c.Predecessor == Predecessor)
                    .Select(c => new ScheduleMyWork
                    {
                        Id = c.Id,
                        FullName = c.FullName,
                        TaskName = c.TaskName,
                        UserDeliverId = c.UserDeliverId,
                        StartDate = c.StartDate,
                        EndDate = c.EndDate,
                        Predecessor = c.Predecessor,
                        MyWorkId = c.MyWorkId,
                        StatusWork = c.StatusWork,
                        DateComplete = c.DateComplete,
                        children = GetChildren(comments, c.Id)
                    }).ToList();
        }
        #endregion
    }
}
public class ScheduleMyWork
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string TaskName { get; set; }
    public int UserDeliverId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? Predecessor { get; set; }
    public string MyWorkId { get; set; }
    public int StatusWork { get; set; }
    public int LevelCv { get; set; }
    public Nullable<DateTime> DateComplete { get; set; }
    public List<ScheduleMyWork> children { get; set; }
}