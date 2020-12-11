using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HumanResource.Application.Notifi;
using HumanResource.Application.Paremeters.Works;
using HumanResource.Data.EF;
using HumanResource.Data.Entities.Works;
using HumanResoureAPI.Common;
using HumanResoureAPI.Common.Systems;
using HumanResoureAPI.Common.WorksCommon;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HumanResoureAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyWorkFlowController : ControllerBase
    {
        private readonly humanDbContext _context;
        private readonly INotifi _notifi;
        public MyWorkFlowController(humanDbContext context, INotifi notifi)
        {
            _context = context;
            _notifi = notifi;
        }
        #region Danh sách công việc chờ phê duyệt thời hạn
        //Post: api/MyWorkFlow/r1GetListWaitSignTime
        [HttpGet]
        [Route("r1GetListWaitSignTime")]
        public async Task<ActionResult<IEnumerator<CV_QT_MyWork>>> r1GetListWaitSignTime()
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var myWorks = from b in _context.CV_QT_WorkFlow
                              join a in _context.CV_QT_MyWork on b.MyWorkId equals a.Id
                              where b.UserDeliverId == userId && b.TypeFlow == 1 && b.Handled != true
                              orderby b.CreateDate descending
                              select new
                              {
                                  MyWorkId = a.Id,
                                  b.Id,
                                  a.Note,
                                  a.Code,
                                  TaskName = "(" + a.TaskCode + ") " + a.TaskName,
                                  a.TaskId,
                                  a.Predecessor,
                                  a.StartDate,
                                  a.EndDate,
                                  b.TypeFlow,
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
                              };
                var qrs = await WorksCommon.Paginate(myWorks, 0, 1000).ToListAsync();

                return new ObjectResult(new { error = 0, data = qrs, total = myWorks.Count() });
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
        #region Danh sách công việc đang thực hiện
        //Post: api/MyWorkFlow/r1GetListPerform
        [HttpGet]
        [Route("r1GetListPerform")]
        public async Task<ActionResult<IEnumerator<CV_QT_MyWork>>> r1GetListPerform()
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var user = await _context.Sys_Dm_User.FindAsync(userId);
                var users = _context.Sys_Dm_User.Where(x => x.ParentDepartId == user.ParentDepartId).Select(c => c.Id);
                var myWorks = from b in _context.CV_QT_WorkFlow
                              join a in _context.CV_QT_MyWork on b.MyWorkId equals a.Id
                              where users.Contains(a.UserTaskId) && b.TypeFlow == 0 && (a.CycleWork == 1 || a.CycleWork == 3)
                              orderby b.CreateDate descending
                              select new
                              {
                                  MyWorkId = a.Id,
                                  b.Id,
                                  a.Note,
                                  a.Code,
                                  TaskName = "(" + a.TaskCode + ") " + a.TaskName,
                                  a.TaskId,
                                  a.Predecessor,
                                  a.StartDate,
                                  a.EndDate,
                                  b.TypeFlow,
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
                              };
                var qrs = await WorksCommon.Paginate(myWorks, 0, 1000).ToListAsync();

                return new ObjectResult(new { error = 0, data = qrs, total = myWorks.Count() });
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
        #region Danh sách công việc chờ phê duyệt hoàn thành
        //Post: api/MyWorkFlow/r1GetListAwaitingComplete
        [HttpGet]
        [Route("r1GetListAwaitingComplete")]
        public async Task<ActionResult<IEnumerator<CV_QT_MyWork>>> r1GetListAwaitingComplete()
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var user = await _context.Sys_Dm_User.FindAsync(userId);
                var users = _context.Sys_Dm_User.Where(x => x.ParentDepartId == user.ParentDepartId).Select(c => c.Id);
                var myWorks = from b in _context.CV_QT_WorkFlow
                              join a in _context.CV_QT_MyWork on b.MyWorkId equals a.Id
                              where b.UserDeliverId == userId && b.Handled == false && (a.CycleWork != 4) && (b.TypeFlow == 4 || b.TypeFlow == 11)
                              orderby b.CreateDate descending
                              select new
                              {
                                  MyWorkId = a.Id,
                                  b.Id,
                                  a.Note,
                                  a.Code,
                                  TaskName = "(" + a.TaskCode + ") " + a.TaskName,
                                  a.TaskId,
                                  a.Predecessor,
                                  a.StartDate,
                                  a.EndDate,
                                  b.TypeFlow,
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
                              };
                var qrs = await WorksCommon.Paginate(myWorks, 0, 1000).ToListAsync();

                return new ObjectResult(new { error = 0, data = qrs, total = myWorks.Count() });
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
        #region Danh sách công việc chờ phê duyệt khởi tạo sau
        //Post: api/MyWorkFlow/r1GetListAwaitingCompleteKts
        [HttpGet]
        [Route("r1GetListAwaitingCompleteKts")]
        public async Task<ActionResult<IEnumerator<CV_QT_MyWork>>> r1GetListAwaitingCompleteKts()
        {
            try
            {
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var user = await _context.Sys_Dm_User.FindAsync(userId);
                var users = _context.Sys_Dm_User.Where(x => x.ParentDepartId == user.ParentDepartId).Select(c => c.Id);
                var myWorks = from b in _context.CV_QT_WorkFlow
                              join a in _context.CV_QT_MyWork on b.MyWorkId equals a.Id
                              where b.UserDeliverId == userId && b.Handled == false && (b.TypeFlow == 14)
                              orderby b.CreateDate descending
                              select new
                              {
                                  MyWorkId = a.Id,
                                  b.Id,
                                  a.Note,
                                  a.Code,
                                  TaskName = "(" + a.TaskCode + ") " + a.TaskName,
                                  a.TaskId,
                                  a.Predecessor,
                                  a.StartDate,
                                  a.EndDate,
                                  b.TypeFlow,
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
                              };
                var qrs = await WorksCommon.Paginate(myWorks, 0, 1000).ToListAsync();

                return new ObjectResult(new { error = 0, data = qrs, total = myWorks.Count() });
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
        #region Trình phê duyệt thời hạn
        //Post: api/MyWorkFlow/r2AddWorkFlowTTH
        [HttpPost, DisableRequestSizeLimit]
        [Route("r2AddWorkFlowTTH")]
        public async Task<ActionResult<IEnumerable<CV_QT_WorkFlow>>> r2AddWorkFlowTTH()
        {
            try
            {
                var modelFlow = JsonConvert.DeserializeObject<FlowModel>(Request.Form["model"]);
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var cV_QT_WorkFlow = await _context.CV_QT_WorkFlow.FirstOrDefaultAsync(x => x.MyWorkId == modelFlow.MyWorkId && x.TypeFlow == 0);
                var cV_QT_MyWork = await _context.CV_QT_MyWork.FirstOrDefaultAsync(x => x.Id == modelFlow.MyWorkId);
                if (cV_QT_WorkFlow.Readed != true)
                {
                    cV_QT_WorkFlow.ReadDate = DateTime.Now;
                    cV_QT_WorkFlow.Readed = true;
                    cV_QT_WorkFlow.Handled = true;
                    cV_QT_WorkFlow.HandleDate = DateTime.Now;
                }
                // lưu quy trình luân chuyển công việc
                foreach (var ktem in modelFlow.UserDelivers)
                {
                    CV_QT_WorkFlow wflow = WorksCommon.objWorkFlow(_context, modelFlow.MyWorkId, userId, ktem.UserId, 1, "CV_THOIHAN", cV_QT_WorkFlow.Id, modelFlow.Note, modelFlow.Require, 1);
                    // lưu thông báo
                    await _notifi.SaveNotifiAsync(2, "CV_THOIHAN", wflow.SendName, "MCV: (" + cV_QT_MyWork.Code.ToString() + ")" + wflow.Note, ktem.UserId, wflow.TypeFlow, "/giaoviec/quytrinhgiaoviec/congviecchophethoihan");
                    // lưu quy trình luân chuyển công việc
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
                            if (modelFlow != null)
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
                }

                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0, ms = "Trình phê duyệt thời hạn hoàn thành thành công!" });

            }
            catch (Exception e)
            {
                bool success = SaveLog.SaveLogEx(_context, "api/MyWorkFlow/r2AddWorkFlowTTH", e.Message, "Trình phê duyệt thời hạn hoàn thành");
                return new ObjectResult(new { error = 1, ms = "Lỗi khi trình phê duyệt thời hạn hoàn thành!" });
            }
        }
        #endregion
        #region Trình giải quyết phối hợp công tác
        //Post: api/MyWorkFlow/r2AddWorkFlowPHCT
        [HttpPost, DisableRequestSizeLimit]
        [Route("r2AddWorkFlowPHCT")]
        public async Task<ActionResult<IEnumerable<CV_QT_WorkFlow>>> r2AddWorkFlowPHCT()
        {
            try
            {
                var modelFlow = JsonConvert.DeserializeObject<FlowModel>(Request.Form["model"]);
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var user = await _context.Sys_Dm_User.FindAsync(userId);
                var cV_QT_WorkFlow = await _context.CV_QT_WorkFlow.Where(x => x.MyWorkId == modelFlow.MyWorkId).OrderByDescending(x => x.CreateDate).FirstOrDefaultAsync(x => x.MyWorkId == modelFlow.MyWorkId && x.TypeFlow == 0);
                var cV_QT_MyWork = await _context.CV_QT_MyWork.FirstOrDefaultAsync(x => x.Id == modelFlow.MyWorkId);
                if (cV_QT_MyWork.Predecessor == null)
                {
                    return new ObjectResult(new { error = 1, ms = "Công việc này không có công việc tiên quyết!" });
                }
                if (cV_QT_WorkFlow.Readed != true)
                {
                    cV_QT_WorkFlow.ReadDate = DateTime.Now;
                    cV_QT_WorkFlow.Readed = true;
                }
                // lưu quy trình luân chuyển công việc
                foreach (var ktem in modelFlow.UserDelivers)
                {
                    CV_QT_WorkFlow wflow = WorksCommon.objWorkFlow(_context, modelFlow.MyWorkId, userId, ktem.UserId, 17, "CV_TRINHXULYPHOIHOP", cV_QT_WorkFlow.Id, modelFlow.Note, modelFlow.Require, 1);
                    // lưu quy trình luân chuyển công việc
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
                            if (modelFlow != null)
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
                }
                // bắt đầu xử lý tính lỗi tự động
                string note = "Hệ thống tự động đánh giá chất lượng chậm tiến độ công việc";
                // người chủ trì
                CV_QT_CounterError cterror = new CV_QT_CounterError();
                cterror.ErrorId = 7;
                cterror.Point = 3;// tinh diem tu dong
                cterror.NguoiBiPhatId = userId;
                cterror.NguoiPhatId = 2028; // Hệ thống
                cterror.MyWorkId = modelFlow.MyWorkId;
                cterror.FlowWorkId = cV_QT_WorkFlow.Id;
                cterror.CreateDate = DateTime.Now;
                cterror.DepartmentId = user.DepartmentId ?? 0;
                _context.CV_QT_CounterError.Add(cterror);
                // công việc tiên quyết
                var mypreWork = _context.CV_QT_MyWork.FirstOrDefault(x => x.Code == cV_QT_MyWork.Predecessor);
                CV_QT_CounterError cterrorph = new CV_QT_CounterError();
                cterrorph.ErrorId = 7;
                cterrorph.Point = 3;
                cterrorph.NguoiBiPhatId = mypreWork.UserTaskId;
                cterrorph.NguoiPhatId = 2028; // Hệ thống
                cterrorph.MyWorkId = modelFlow.MyWorkId;
                cterrorph.FlowWorkId = cV_QT_WorkFlow.Id;
                cterrorph.CreateDate = DateTime.Now;
                cterrorph.DepartmentId = user.DepartmentId ?? 0;
                _context.CV_QT_CounterError.Add(cterror);

                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0, ms = "Trình giải quyết phối hợp công tác thành công!" });

            }
            catch (Exception e)
            {
                bool success = SaveLog.SaveLogEx(_context, "api/MyWorkFlow/r2AddWorkFlowTTH", e.Message, "Trình giải quyết phối hợp công tác");
                return new ObjectResult(new { error = 1, ms = "Lỗi khi trình giải quyết phối hợp công tác!" });
            }
        }
        #endregion
        #region Trình phê duyệt hoàn thành công việc
        //Post: api/MyWorkFlow/r2AddWorkFlowHTCV
        [HttpPost, DisableRequestSizeLimit]
        [Route("r2AddWorkFlowHTCV")]
        public async Task<ActionResult<IEnumerable<CV_QT_WorkFlow>>> r2AddWorkFlowHTCV()
        {
            try
            {
                var modelFlow = JsonConvert.DeserializeObject<FlowModel>(Request.Form["model"]);
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var user = await _context.Sys_Dm_User.FindAsync(userId);
                var workFlows = _context.CV_QT_WorkFlow.Where(x => x.MyWorkId == modelFlow.MyWorkId).Select(x => x.TypeFlow).Distinct().ToList();
                CV_QT_WorkFlow cV_QT_WorkFlow = new CV_QT_WorkFlow();
                if (!workFlows.Contains(5)) // nếu không tồn tại yêu cầu chỉnh sửa trong quy trình
                {
                    cV_QT_WorkFlow = await _context.CV_QT_WorkFlow.FirstOrDefaultAsync(x => x.MyWorkId == modelFlow.MyWorkId && x.TypeFlow == 0);
                }
                else
                {
                    cV_QT_WorkFlow = await _context.CV_QT_WorkFlow.FirstOrDefaultAsync(x => x.MyWorkId == modelFlow.MyWorkId && x.TypeFlow == 5);
                }


                if (cV_QT_WorkFlow.Readed != true)
                {
                    cV_QT_WorkFlow.ReadDate = DateTime.Now;
                    cV_QT_WorkFlow.Readed = true;
                    cV_QT_WorkFlow.Handled = true;
                    cV_QT_WorkFlow.HandleDate = DateTime.Now;
                }

                var cV_QT_MyWork = await _context.CV_QT_MyWork.FindAsync(modelFlow.MyWorkId); // update trạng thái và thời gian tạm tính hoàn thành công việc
                if (cV_QT_MyWork != null)
                {
                    if (cV_QT_MyWork.CycleWork == 0)
                    {
                        return new ObjectResult(new { error = 1, ms = "Công việc không thể trình hoàn thành vì chưa được thực hiện!" });
                    }
                    // bắt đầu xử lý dừng công việc lưu vào bảng nhật ký và lịch sử
                    var workNote = await _context.CV_QT_WorkNote.FirstOrDefaultAsync(x => x.DateEnd == null && x.MyWorkId == modelFlow.MyWorkId); // truy vấn bảng nhật ký công việc
                    bool success = WorksCommon.PauseMyWork(_context, cV_QT_MyWork, workNote); // true là đã lưu thành công, false là không thuộc trường hợp cần lưu
                    // kết thúc xử lý tạm dừng công việc

                    cV_QT_MyWork.CompleteDate = DateTime.Now;
                    cV_QT_MyWork.TypeComplete = 1;
                    cV_QT_MyWork.CycleWork = 2;

                }
                // chuyển cho người thực hiện công việc tiếp theo
                if (modelFlow.UserNextId != null)
                {
                    CV_QT_WorkFlow wflowNext = WorksCommon.objWorkFlow(_context, modelFlow.MyWorkId, userId, modelFlow.UserNextId ?? 0, 11, "CV_KETQUA", cV_QT_WorkFlow.Id, modelFlow.Note, modelFlow.Require, 2);

                    _context.CV_QT_WorkFlow.Add(wflowNext);
                    // lưu thông báo cho người nhận
                    await _notifi.SaveNotifiAsync(2, "CV_KETQUA", wflowNext.SendName, "MCV: (" + cV_QT_MyWork.Code.ToString() + ")" + wflowNext.Note, modelFlow.UserNextId ?? 0, wflowNext.TypeFlow, "/giaoviec/quytrinhgiaoviec/congviecchophehoanthanh");
                    // lưu thông báo cho người nhận
                    if (Request.Form.Files.Count != 0)
                    {
                        foreach (var item in Request.Form.Files)
                        {
                            CV_QT_WorkFlowFile obj = new CV_QT_WorkFlowFile();
                            var file = item;
                            var folderName = Path.Combine("Resources", "WorkFlows", "Results");
                            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                            if (!Directory.Exists(pathToSave))
                            {
                                Directory.CreateDirectory(pathToSave);
                            }
                            if (modelFlow != null)
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
                            obj.WorkFlowId = wflowNext.Id;
                            obj.Size = file.Length / 1048576;

                            _context.CV_QT_WorkFlowFile.Add(obj);
                        }

                    }
                }
                // chuyển cho lãnh đạo đơn vị phê duyệt hoàn thành
                if (modelFlow.UserManagerId != null)
                {
                    CV_QT_WorkFlow wflowMana = WorksCommon.objWorkFlow(_context, modelFlow.MyWorkId, userId, modelFlow.UserManagerId ?? 0, 4, "CV_KETQUA", cV_QT_WorkFlow.Id, modelFlow.Note, modelFlow.Require, 1);
                    _context.CV_QT_WorkFlow.Add(wflowMana);
                    await _notifi.SaveNotifiAsync(2, "CV_KETQUA", wflowMana.SendName, "MCV: (" + cV_QT_MyWork.Code.ToString() + ")" + wflowMana.Note, modelFlow.UserManagerId ?? 0, wflowMana.TypeFlow, "/giaoviec/quytrinhgiaoviec/congviecchophehoanthanh");

                    if (Request.Form.Files.Count != 0)
                    {
                        foreach (var item in Request.Form.Files)
                        {
                            CV_QT_WorkFlowFile obj = new CV_QT_WorkFlowFile();
                            var file = item;
                            var folderName = Path.Combine("Resources", "WorkFlows", "Results");
                            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                            if (!Directory.Exists(pathToSave))
                            {
                                Directory.CreateDirectory(pathToSave);
                            }
                            if (modelFlow != null)
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
                            obj.WorkFlowId = wflowMana.Id;
                            obj.Size = file.Length / 1048576;

                            _context.CV_QT_WorkFlowFile.Add(obj);
                        }

                    }
                }
                // lưu quy trình luân chuyển công việc cho người theo dõi
                foreach (var ktem in modelFlow.UserDelivers)
                {
                    CV_QT_WorkFlow wflow = WorksCommon.objWorkFlow(_context, modelFlow.MyWorkId, userId, ktem.UserId, 12, "CV_THEODOI", cV_QT_WorkFlow.Id, modelFlow.Note, modelFlow.Require, 3);
                    await _notifi.SaveNotifiAsync(2, "CV_KETQUA", wflow.SendName, "MCV: (" + cV_QT_MyWork.Code.ToString() + ")" + wflow.Note, ktem.UserId, wflow.TypeFlow, "/giaoviec/quytrinhgiaoviec/congviecchophehoanthanh");

                    // lưu quy trình luân chuyển công việc
                    _context.CV_QT_WorkFlow.Add(wflow);
                    if (Request.Form.Files.Count != 0)
                    {
                        foreach (var item in Request.Form.Files)
                        {
                            CV_QT_WorkFlowFile obj = new CV_QT_WorkFlowFile();
                            var file = item;
                            var folderName = Path.Combine("Resources", "WorkFlows", "Results");
                            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                            if (!Directory.Exists(pathToSave))
                            {
                                Directory.CreateDirectory(pathToSave);
                            }
                            if (modelFlow != null)
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
                }

                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0, ms = "Trình phê duyệt hoàn thành công việc thành công!" });

            }
            catch (Exception e)
            {
                return new ObjectResult(new { error = 1, ms = "Lỗi khi trình phê duyệt thời hạn hoàn thành!" });
            }
        }
        #endregion
        #region Trình phê duyệt hoàn thành công việc khởi tạo sau
        //Post: api/MyWorkFlow/r2AddWorkFlowHTCVKTS
        [HttpPost, DisableRequestSizeLimit]
        [Route("r2AddWorkFlowHTCVKTS")]
        public async Task<ActionResult<IEnumerable<CV_QT_WorkFlow>>> r2AddWorkFlowHTCVKTS()
        {
            try
            {
                var modelFlow = JsonConvert.DeserializeObject<FlowModel>(Request.Form["model"]);
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var user = await _context.Sys_Dm_User.FindAsync(userId);
                var workFlows = _context.CV_QT_WorkFlow.Where(x => x.MyWorkId == modelFlow.MyWorkId).Select(x => x.TypeFlow).Distinct().ToList();
                CV_QT_WorkFlow cV_QT_WorkFlow = new CV_QT_WorkFlow();
                cV_QT_WorkFlow = await _context.CV_QT_WorkFlow.FirstOrDefaultAsync(x => x.MyWorkId == modelFlow.MyWorkId && x.TypeFlow == 13);



                if (cV_QT_WorkFlow.Readed != true)
                {
                    cV_QT_WorkFlow.ReadDate = DateTime.Now;
                    cV_QT_WorkFlow.Readed = true;
                    cV_QT_WorkFlow.Handled = true;
                    cV_QT_WorkFlow.HandleDate = DateTime.Now;
                }

                var cV_QT_MyWork = await _context.CV_QT_MyWork.FindAsync(modelFlow.MyWorkId); // update trạng thái và thời gian tạm tính hoàn thành công việc
                if (cV_QT_MyWork != null)
                {
                    if (cV_QT_MyWork.CycleWork == 0)
                    {
                        return new ObjectResult(new { error = 1, ms = "Công việc không thể trình hoàn thành vì chưa được thực hiện!" });
                    }
                    // bắt đầu xử lý dừng công việc lưu vào bảng nhật ký và lịch sử
                    var workNote = await _context.CV_QT_WorkNote.FirstOrDefaultAsync(x => x.DateEnd == null && x.MyWorkId == modelFlow.MyWorkId); // truy vấn bảng nhật ký công việc
                    bool success = WorksCommon.PauseMyWork(_context, cV_QT_MyWork, workNote); // true là đã lưu thành công, false là không thuộc trường hợp cần lưu
                    // kết thúc xử lý tạm dừng công việc

                    cV_QT_MyWork.CompleteDate = DateTime.Now;
                    cV_QT_MyWork.TypeComplete = 1;
                    cV_QT_MyWork.CycleWork = 2;
                }
                // chuyển cho lãnh đạo đơn vị phê duyệt hoàn thành
                if (modelFlow.UserManagerId != null)
                {
                    CV_QT_WorkFlow wflowMana = WorksCommon.objWorkFlow(_context, modelFlow.MyWorkId, userId, modelFlow.UserManagerId ?? 0, 14, "CV_KETQUA", cV_QT_WorkFlow.Id, modelFlow.Note, "Trình phê duyệt khởi tạo công việc sau", 1);
                    _context.CV_QT_WorkFlow.Add(wflowMana);
                    await _notifi.SaveNotifiAsync(2, "CV_KETQUA", wflowMana.SendName, "MCV: (" + cV_QT_MyWork.Code.ToString() + ")" + wflowMana.Note, modelFlow.UserManagerId ?? 0, wflowMana.TypeFlow, "/giaoviec/quytrinhgiaoviec/congviecchophehoanthanh");

                    if (Request.Form.Files.Count != 0)
                    {
                        foreach (var item in Request.Form.Files)
                        {
                            CV_QT_WorkFlowFile obj = new CV_QT_WorkFlowFile();
                            var file = item;
                            var folderName = Path.Combine("Resources", "WorkFlows", "Results");
                            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                            if (!Directory.Exists(pathToSave))
                            {
                                Directory.CreateDirectory(pathToSave);
                            }
                            if (modelFlow != null)
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
                            obj.WorkFlowId = wflowMana.Id;
                            obj.Size = file.Length / 1048576;

                            _context.CV_QT_WorkFlowFile.Add(obj);
                        }

                    }
                }
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0, ms = "Trình phê duyệt hoàn thành công việc thành công!" });

            }
            catch (Exception e)
            {
                return new ObjectResult(new { error = 1, ms = "Lỗi khi trình phê duyệt thời hạn hoàn thành!" });
            }
        }
        #endregion
        #region Phê duyệt thời hạn
        //Post: api/MyWorkFlow/r2AddWorkFlowDuyetTH
        [HttpPost, DisableRequestSizeLimit]
        [Route("r2AddWorkFlowDuyetTH")]
        public async Task<ActionResult<IEnumerable<CV_QT_WorkFlow>>> r2AddWorkFlowDuyetTH()
        {
            try
            {
                var modelFlow = JsonConvert.DeserializeObject<FlowModel>(Request.Form["model"]);
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var cV_QT_WorkFlow = await _context.CV_QT_WorkFlow.FirstOrDefaultAsync(x => x.MyWorkId == modelFlow.MyWorkId && x.TypeFlow == 1 && x.UserDeliverId == userId);
                var cV_QT_MyWork = await _context.CV_QT_MyWork.FirstOrDefaultAsync(x => x.Id == modelFlow.MyWorkId);
                if (cV_QT_WorkFlow.Handled != true)
                {
                    cV_QT_WorkFlow.Handled = true;
                    cV_QT_WorkFlow.HandleDate = DateTime.Now;
                }

                var mywork = await _context.CV_QT_MyWork.FindAsync(modelFlow.MyWorkId);
                int TypeFlow = 2;
                var datee = TransforDate.FromDoubleToDate(modelFlow.DEnd ?? 0);
                var dates = TransforDate.FromDoubleToDate(modelFlow.DStart ?? 0);
                if (dates.Date == mywork.EndDate.Value.Date && dates.Hour == mywork.EndDate.Value.Hour && dates.Minute == mywork.EndDate.Value.Minute &&
                    datee.Date == mywork.ExpectedDate.Value.Date && dates.Hour == mywork.ExpectedDate.Value.Hour && dates.Minute == mywork.ExpectedDate.Value.Minute
                    )
                {
                    TypeFlow = 3;
                }
                else
                {
                    mywork.EndDate = TransforDate.FromDoubleToDate(modelFlow.DEnd ?? 0);
                    mywork.ExpectedDate = TransforDate.FromDoubleToDate(modelFlow.DStart ?? 0);
                    TypeFlow = 2;
                }
                // lưu quy trình luân chuyển công việc

                CV_QT_WorkFlow wflow = WorksCommon.objWorkFlow(_context, modelFlow.MyWorkId, userId, cV_QT_WorkFlow.UserSendId, TypeFlow, "CV_THOIHAN", cV_QT_WorkFlow.Id, modelFlow.Note, modelFlow.Require, 1);

                // lưu quy trình luân chuyển công việc
                _context.CV_QT_WorkFlow.Add(wflow);
                // lưu thông báo cho người nhận
                await _notifi.SaveNotifiAsync(2, "CV_THOIHAN", wflow.SendName, "MCV: (" + cV_QT_MyWork.Code.ToString() + ")" + wflow.Note, cV_QT_WorkFlow.UserSendId, wflow.TypeFlow, "/giaoviec/quytrinhgiaoviec/congvieccuatoi");
                // lưu thông báo cho người nhận
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
                        if (modelFlow != null)
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
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0, ms = "Phê duyệt thời hạn hoàn thành thành công!" });

            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1, ms = "Lỗi khi phê duyệt thời hạn hoàn thành!" });
            }
        }
        #endregion
        #region Duyệt phê duyệt hoàn thành công việc
        //Post: api/MyWorkFlow/r2AddWorkFlowDuyetHTCV
        [HttpPost, DisableRequestSizeLimit]
        [Route("r2AddWorkFlowDuyetHTCV")]
        public async Task<ActionResult<IEnumerable<CV_QT_WorkFlow>>> r2AddWorkFlowDuyetHTCV()
        {
            try
            {
                var modelFlow = JsonConvert.DeserializeObject<FlowModel>(Request.Form["model"]);
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var cV_QT_WorkFlow = await _context.CV_QT_WorkFlow.FirstOrDefaultAsync(x => x.MyWorkId == modelFlow.MyWorkId && x.TypeFlow == 4 && x.Handled == false); // lọc thông tin công việc đnag được trình duyệt hoàn thành
                if (cV_QT_WorkFlow.Readed != true)
                {
                    cV_QT_WorkFlow.ReadDate = DateTime.Now;
                    cV_QT_WorkFlow.Readed = true;
                }
                if (cV_QT_WorkFlow.Handled != true)
                {
                    cV_QT_WorkFlow.Handled = true;
                    cV_QT_WorkFlow.HandleDate = DateTime.Now;
                }

                var cV_QT_MyWork = await _context.CV_QT_MyWork.FindAsync(modelFlow.MyWorkId); // update trạng thái hoàn thành công việc
                if (cV_QT_MyWork != null)
                {
                    var user = await _context.Sys_Dm_User.FindAsync(cV_QT_MyWork.UserTaskId);
                    cV_QT_MyWork.TypeComplete = 3;
                    cV_QT_MyWork.CycleWork = 4;
                    if (cV_QT_MyWork.CompleteDate.Value > cV_QT_MyWork.EndDate.Value) // nếu quán hạn khi trình hoàn thành công việc, hệ thống tự động tính lỗi
                    {
                        // bắt đầu xử lý tính lỗi tự động
                        string note = "Hệ thống tự động đánh giá chất lượng chậm tiến độ công việc";
                        var error = await _context.CV_DM_Error.FindAsync(1);
                        CV_QT_CounterError cterror = new CV_QT_CounterError();
                        cterror.ErrorId = error.Id;
                        cterror.Point = WorksCommon.countPointAuto(cV_QT_MyWork.EndDate ?? DateTime.Now, cV_QT_MyWork.CompleteDate ?? DateTime.Now, cV_QT_MyWork.PointTime ?? 0.0);// tinh diem tu dong
                        cterror.NguoiBiPhatId = cV_QT_MyWork.UserTaskId;
                        cterror.NguoiPhatId = 2028; // Hệ thống
                        cterror.MyWorkId = modelFlow.MyWorkId;
                        cterror.FlowWorkId = cV_QT_WorkFlow.Id;
                        cterror.CreateDate = DateTime.Now;
                        cterror.DepartmentId = user.DepartmentId ?? 0;
                        _context.CV_QT_CounterError.Add(cterror);
                        CV_QT_WorkFlow wflowerror = WorksCommon.objWorkFlow(_context, modelFlow.MyWorkId, 2028, userId, 10, "CV_DANHGIACL", cV_QT_WorkFlow.Id, note, modelFlow.Require, 1);
                        _context.CV_QT_WorkFlow.Add(wflowerror);
                        // kết thúc tính lỗi tự động
                    }
                    CV_QT_WorkFlow wflow2 = WorksCommon.objWorkFlow(_context, modelFlow.MyWorkId, userId, cV_QT_MyWork.UserTaskId, 6, "CV_KETQUA", cV_QT_WorkFlow.Id, modelFlow.Note, modelFlow.Require, 1); // chuyển hoàn thành công việc
                    _context.CV_QT_WorkFlow.Add(wflow2);
                    // lưu thông báo cho người nhận
                    await _notifi.SaveNotifiAsync(2, "CV_KETQUA", wflow2.SendName, "MCV: (" + cV_QT_MyWork.Code.ToString() + ")" + wflow2.Note, cV_QT_MyWork.UserTaskId, wflow2.TypeFlow, "/giaoviec/quytrinhgiaoviec/congvieccuatoi");
                    // lưu thông báo cho người thực hiện công việc tiếp theo nếu có

                    await _notifi.SaveNotifiNextAsync(cV_QT_MyWork.Code, 2, "CV_KETQUA", wflow2.DeliverName, "MCV: (" + cV_QT_MyWork.Code.ToString() + ")" + wflow2.Note, wflow2.TypeFlow, "/giaoviec/quytrinhgiaoviec/congvieccuatoi");
                    // lưu thông báo cho người nhận
                    if (Request.Form.Files.Count != 0)
                    {
                        foreach (var item in Request.Form.Files)
                        {
                            CV_QT_WorkFlowFile obj = new CV_QT_WorkFlowFile();
                            var file = item;
                            var folderName = Path.Combine("Resources", "WorkFlows", "Results");
                            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                            if (!Directory.Exists(pathToSave))
                            {
                                Directory.CreateDirectory(pathToSave);
                            }
                            if (modelFlow != null)
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
                            obj.Name = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                            obj.WorkFlowId = wflow2.Id;
                            obj.Size = file.Length / 1048576;

                            _context.CV_QT_WorkFlowFile.Add(obj);
                        }
                    }
                }
                // luu quy trình cho người chủ trì

                // lưu quy trình luân chuyển công việc cho người theo dõi
                foreach (var ktem in modelFlow.UserDelivers)
                {
                    CV_QT_WorkFlow wflow = WorksCommon.objWorkFlow(_context, modelFlow.MyWorkId, userId, ktem.UserId, 12, "CV_THEODOI", cV_QT_WorkFlow.Id, modelFlow.Note, modelFlow.Require, 3);
                    // lưu quy trình luân chuyển công việc
                    _context.CV_QT_WorkFlow.Add(wflow);
                    // lưu thông báo cho người nhận
                    await _notifi.SaveNotifiAsync(2, "CV_THEODOI", wflow.SendName, wflow.Note, ktem.UserId, wflow.TypeFlow, "/giaoviec/quytrinhgiaoviec/congvieccuatoi");
                    // lưu thông báo cho người nhận

                    if (Request.Form.Files.Count != 0)
                    {
                        foreach (var item in Request.Form.Files)
                        {
                            CV_QT_WorkFlowFile obj = new CV_QT_WorkFlowFile();
                            var file = item;
                            var folderName = Path.Combine("Resources", "WorkFlows", "Results");
                            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                            if (!Directory.Exists(pathToSave))
                            {
                                Directory.CreateDirectory(pathToSave);
                            }
                            if (modelFlow != null)
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
                }

                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0, ms = "Phê duyệt hoàn thành công việc thành công!" });

            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1, ms = "Lỗi khi phê duyệt hoàn thành công việc!" });
            }
        }
        #endregion
        #region Duyệt phê duyệt phối hợp công tác (chỉ đạo xử lý)
        //Post: api/MyWorkFlow/r2AddWorkFlowXuLyPHCT
        [HttpPost, DisableRequestSizeLimit]
        [Route("r2AddWorkFlowXuLyPHCT")]
        public async Task<ActionResult<IEnumerable<CV_QT_WorkFlow>>> r2AddWorkFlowXuLyPHCT()
        {
            try
            {
                var modelFlow = JsonConvert.DeserializeObject<FlowModel>(Request.Form["model"]);
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var cV_QT_WorkFlow = await _context.CV_QT_WorkFlow.FirstOrDefaultAsync(x => x.MyWorkId == modelFlow.MyWorkId && x.TypeFlow == 17 && x.Handled == false); // lọc thông tin công việc đnag được trình phối hợp công tác
                if (cV_QT_WorkFlow.Readed != true)
                {
                    cV_QT_WorkFlow.ReadDate = DateTime.Now;
                    cV_QT_WorkFlow.Readed = true;
                }
                if (cV_QT_WorkFlow.Handled != true)
                {
                    cV_QT_WorkFlow.Handled = true;
                    cV_QT_WorkFlow.HandleDate = DateTime.Now;
                }

                var cV_QT_MyWork = await _context.CV_QT_MyWork.FindAsync(modelFlow.MyWorkId); // update trạng thái hoàn thành công việc
                if (cV_QT_MyWork != null)
                {
  
                    CV_QT_WorkFlow wflow2 = WorksCommon.objWorkFlow(_context, modelFlow.MyWorkId, userId, cV_QT_MyWork.UserTaskId, 18, "CV_CHIDAOXULY", cV_QT_WorkFlow.Id, modelFlow.Note, modelFlow.Require, 1); // chuyển hoàn thành công việc
                    _context.CV_QT_WorkFlow.Add(wflow2);
                    // lưu thông báo cho người nhận
                    await _notifi.SaveNotifiAsync(2, "CV_CHIDAOXULY", wflow2.SendName, "MCV: (" + cV_QT_MyWork.Code.ToString() + ")" + wflow2.Note, cV_QT_MyWork.UserTaskId, wflow2.TypeFlow, "/giaoviec/quytrinhgiaoviec/congvieccuatoi");
                    // lưu thông báo cho người nhận
                    if (Request.Form.Files.Count != 0)
                    {
                        foreach (var item in Request.Form.Files)
                        {
                            CV_QT_WorkFlowFile obj = new CV_QT_WorkFlowFile();
                            var file = item;
                            var folderName = Path.Combine("Resources", "WorkFlows", "Results");
                            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                            if (!Directory.Exists(pathToSave))
                            {
                                Directory.CreateDirectory(pathToSave);
                            }
                            if (modelFlow != null)
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
                            obj.Name = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                            obj.WorkFlowId = wflow2.Id;
                            obj.Size = file.Length / 1048576;

                            _context.CV_QT_WorkFlowFile.Add(obj);
                        }
                    }
                }
                // luu quy trình cho người chủ trì
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0, ms = "Phê duyệt hoàn thành công việc thành công!" });

            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1, ms = "Lỗi khi phê duyệt hoàn thành công việc!" });
            }
        }
        #endregion
        #region Duyệt đồng ý khởi tạo sau
        //Post: api/MyWorkFlow/r2AddWorkFlowDongyKTS
        [HttpPost, DisableRequestSizeLimit]
        [Route("r2AddWorkFlowDongyKTS")]
        public async Task<ActionResult<IEnumerable<CV_QT_WorkFlow>>> r2AddWorkFlowDongyKTS()
        {
            try
            {
                var modelFlow = JsonConvert.DeserializeObject<FlowModel>(Request.Form["model"]);
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var cV_QT_WorkFlow = await _context.CV_QT_WorkFlow.FirstOrDefaultAsync(x => x.MyWorkId == modelFlow.MyWorkId && x.TypeFlow == 14 && x.Handled == false); // lọc thông tin công việc đnag được trình duyệt hoàn thành
                if (cV_QT_WorkFlow.Readed != true)
                {
                    cV_QT_WorkFlow.ReadDate = DateTime.Now;
                    cV_QT_WorkFlow.Readed = true;
                }
                if (cV_QT_WorkFlow.Handled != true)
                {
                    cV_QT_WorkFlow.Handled = true;
                    cV_QT_WorkFlow.HandleDate = DateTime.Now;
                }

                var cV_QT_MyWork = await _context.CV_QT_MyWork.FindAsync(modelFlow.MyWorkId); // update trạng thái hoàn thành công việc
                if (cV_QT_MyWork != null)
                {
                    cV_QT_MyWork.TypeComplete = 3;
                    cV_QT_MyWork.CycleWork = 4;
                    // thêm vào Note
                    CV_QT_WorkNote note = new CV_QT_WorkNote()
                    {
                        MyWorkId = cV_QT_MyWork.Id,
                        DateStart = cV_QT_MyWork.StartDate,
                        DateEnd = cV_QT_MyWork.EndDate ?? DateTime.Now,
                        WorkTime = cV_QT_MyWork.WorkTime ?? 0,
                        OverTime = false,
                        CreatedBy = cV_QT_MyWork.UserTaskId,
                        State = 0
                    };
                    _context.CV_QT_WorkNote.Add(note);
                    CV_QT_WorkFlow wflow2 = WorksCommon.objWorkFlow(_context, modelFlow.MyWorkId, userId, cV_QT_MyWork.UserTaskId, 16, "CV_KETQUA", cV_QT_WorkFlow.Id, modelFlow.Note, modelFlow.Require, 1); // chuyển hoàn thành công việc
                    _context.CV_QT_WorkFlow.Add(wflow2);
                    // lưu thông báo cho người nhận
                    await _notifi.SaveNotifiAsync(2, "CV_KETQUA", wflow2.SendName, "MCV: (" + cV_QT_MyWork.Code.ToString() + ")" + wflow2.Note, cV_QT_MyWork.UserTaskId, wflow2.TypeFlow, "/giaoviec/quytrinhgiaoviec/congvieckhoitaosau");
                    if (Request.Form.Files.Count != 0)
                    {
                        foreach (var item in Request.Form.Files)
                        {
                            CV_QT_WorkFlowFile obj = new CV_QT_WorkFlowFile();
                            var file = item;
                            var folderName = Path.Combine("Resources", "WorkFlows", "Results");
                            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                            if (!Directory.Exists(pathToSave))
                            {
                                Directory.CreateDirectory(pathToSave);
                            }
                            if (modelFlow != null)
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
                            obj.Name = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                            obj.WorkFlowId = wflow2.Id;
                            obj.Size = file.Length / 1048576;

                            _context.CV_QT_WorkFlowFile.Add(obj);
                        }
                    }
                }
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0, ms = "Đồng ý công việc khởi tạo sau thành công!" });

            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1, ms = "Lỗi khi đồng ý công việc khởi tạo sau!" });
            }
        }
        #endregion
        #region Duyệt không đồng ý khởi tạo sau
        //Post: api/MyWorkFlow/r2AddWorkFlowNotDongyKTS
        [HttpPost, DisableRequestSizeLimit]
        [Route("r2AddWorkFlowNotDongyKTS")]
        public async Task<ActionResult<IEnumerable<CV_QT_WorkFlow>>> r2AddWorkFlowNotDongyKTS()
        {
            try
            {
                var modelFlow = JsonConvert.DeserializeObject<FlowModel>(Request.Form["model"]);
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var cV_QT_WorkFlow = await _context.CV_QT_WorkFlow.FirstOrDefaultAsync(x => x.MyWorkId == modelFlow.MyWorkId && x.TypeFlow == 14 && x.Handled == false); // lọc thông tin công việc đnag được trình duyệt hoàn thành
                if (cV_QT_WorkFlow.Readed != true)
                {
                    cV_QT_WorkFlow.ReadDate = DateTime.Now;
                    cV_QT_WorkFlow.Readed = true;
                }
                if (cV_QT_WorkFlow.Handled != true)
                {
                    cV_QT_WorkFlow.Handled = true;
                    cV_QT_WorkFlow.HandleDate = DateTime.Now;
                }

                var cV_QT_MyWork = await _context.CV_QT_MyWork.FindAsync(modelFlow.MyWorkId); // update trạng thái hoàn thành công việc
                if (cV_QT_MyWork != null)
                {
                    cV_QT_MyWork.TypeComplete = 1;
                    cV_QT_MyWork.CycleWork = 4;
                    CV_QT_WorkFlow wflow2 = WorksCommon.objWorkFlow(_context, modelFlow.MyWorkId, userId, cV_QT_MyWork.UserTaskId, 15, "CV_KETQUA", cV_QT_WorkFlow.Id, modelFlow.Note, modelFlow.Require, 1); // chuyển hoàn thành công việc
                    _context.CV_QT_WorkFlow.Add(wflow2);
                    // lưu thông báo cho người nhận
                    await _notifi.SaveNotifiAsync(2, "CV_KETQUA", wflow2.SendName, "MCV: (" + cV_QT_MyWork.Code.ToString() + ")" + wflow2.Note, cV_QT_MyWork.UserTaskId, wflow2.TypeFlow, "/giaoviec/quytrinhgiaoviec/congvieckhoitaosau");
                    if (Request.Form.Files.Count != 0)
                    {
                        foreach (var item in Request.Form.Files)
                        {
                            CV_QT_WorkFlowFile obj = new CV_QT_WorkFlowFile();
                            var file = item;
                            var folderName = Path.Combine("Resources", "WorkFlows", "Results");
                            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                            if (!Directory.Exists(pathToSave))
                            {
                                Directory.CreateDirectory(pathToSave);
                            }
                            if (modelFlow != null)
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
                            obj.Name = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                            obj.WorkFlowId = wflow2.Id;
                            obj.Size = file.Length / 1048576;

                            _context.CV_QT_WorkFlowFile.Add(obj);
                        }
                    }
                }
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0, ms = "Không đồng ý công việc khởi tạo sau thành công!" });

            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1, ms = "Lỗi khi không đồng ý công việc khởi tạo sau!" });
            }
        }
        #endregion
        #region Yêu cầu chỉnh sửa khi trình duyệt hoàn thành
        //Post: api/MyWorkFlow/r2AddWorkFlowYeuCauChinhSua
        [HttpPost, DisableRequestSizeLimit]
        [Route("r2AddWorkFlowYeuCauChinhSua")]
        public async Task<ActionResult<IEnumerable<CV_QT_WorkFlow>>> r2AddWorkFlowYeuCauChinhSua()
        {
            try
            {
                var modelFlow = JsonConvert.DeserializeObject<FlowModel>(Request.Form["model"]);
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var user = await _context.Sys_Dm_User.FindAsync(userId);
                var cV_QT_WorkFlow = await _context.CV_QT_WorkFlow.FirstOrDefaultAsync(x => x.MyWorkId == modelFlow.MyWorkId && x.TypeFlow == 4); // lọc thông tin công việc đnag được trình duyệt hoàn thành
                if (cV_QT_WorkFlow.Handled != true)
                {
                    cV_QT_WorkFlow.Handled = true;
                    cV_QT_WorkFlow.HandleDate = DateTime.Now;
                }

                var cV_QT_MyWork = await _context.CV_QT_MyWork.FindAsync(modelFlow.MyWorkId); // update trạng thái và thời gian tạm tính hoàn thành công việc
                if (cV_QT_MyWork != null)
                {
                    cV_QT_MyWork.TypeComplete = 0;
                    if (modelFlow.DEnd != null)
                    {
                        cV_QT_MyWork.EndDate = TransforDate.FromDoubleToDate(modelFlow.DEnd ?? 0);
                        await WorksCommon.saveDateChangeMyWork(_context, modelFlow.MyWorkId, cV_QT_MyWork.StartDate ?? DateTime.Now, cV_QT_MyWork.EndDate ?? DateTime.Now, userId);
                    }

                    cV_QT_MyWork.CycleWork = 2;
                    cV_QT_MyWork.CompleteDate = null;
                    // luu lai thay doi ve thoi gian ket thuc cong viec

                    string note = "Hệ thống tự động đánh giá chất lượng công việc bị yêu cầu chỉnh sửa";
                    var error = await _context.CV_DM_Error.FindAsync(5);
                    CV_QT_CounterError cterror = new CV_QT_CounterError();
                    cterror.ErrorId = error.Id;
                    cterror.Point = error.Point;
                    cterror.NguoiBiPhatId = cV_QT_MyWork.UserTaskId;
                    cterror.NguoiPhatId = 2028; // Hệ thống
                    cterror.MyWorkId = modelFlow.MyWorkId;
                    cterror.FlowWorkId = cV_QT_WorkFlow.Id;
                    cterror.CreateDate = DateTime.Now;
                    cterror.DepartmentId = user.DepartmentId ?? 0;
                    _context.CV_QT_CounterError.Add(cterror);
                    CV_QT_WorkFlow wflowerror = WorksCommon.objWorkFlow(_context, modelFlow.MyWorkId, 2028, cV_QT_MyWork.UserTaskId, 10, "CV_DANHGIACL", cV_QT_WorkFlow.Id, note, modelFlow.Require, 1);
                    _context.CV_QT_WorkFlow.Add(wflowerror);

                    CV_QT_WorkFlow wflow2 = WorksCommon.objWorkFlow(_context, modelFlow.MyWorkId, userId, cV_QT_MyWork.UserTaskId, 5, "CV_YEUCAUCHINHSUA", cV_QT_WorkFlow.Id, modelFlow.Note, modelFlow.Require, 1); // chuyển hoàn thành công việc
                    _context.CV_QT_WorkFlow.Add(wflow2);
                    await _notifi.SaveNotifiAsync(2, "CV_YEUCAUCHINHSUA", wflow2.SendName, "MCV: (" + cV_QT_MyWork.Code.ToString() + ")" + wflow2.Note, wflow2.UserDeliverId, wflow2.TypeFlow, "/giaoviec/quytrinhgiaoviec/congvieccuatoi");
                    if (Request.Form.Files.Count != 0)
                    {
                        foreach (var item in Request.Form.Files)
                        {
                            CV_QT_WorkFlowFile obj = new CV_QT_WorkFlowFile();
                            var file = item;
                            var folderName = Path.Combine("Resources", "WorkFlows", "Results");
                            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                            if (!Directory.Exists(pathToSave))
                            {
                                Directory.CreateDirectory(pathToSave);
                            }
                            if (modelFlow != null)
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
                            obj.WorkFlowId = wflow2.Id;
                            obj.Size = Convert.ToDouble(file.Length / 1048576.0);

                            _context.CV_QT_WorkFlowFile.Add(obj);
                        }
                    }
                }
                // luu quy trình cho người chủ trì

                // lưu quy trình luân chuyển công việc cho người theo dõi
                foreach (var ktem in modelFlow.UserDelivers)
                {
                    CV_QT_WorkFlow wflow = WorksCommon.objWorkFlow(_context, modelFlow.MyWorkId, userId, ktem.UserId, 12, "CV_THEODOI", cV_QT_WorkFlow.Id, modelFlow.Note, modelFlow.Require, 3);
                    // lưu quy trình luân chuyển công việc
                    _context.CV_QT_WorkFlow.Add(wflow);
                    if (Request.Form.Files.Count != 0)
                    {
                        foreach (var item in Request.Form.Files)
                        {
                            CV_QT_WorkFlowFile obj = new CV_QT_WorkFlowFile();
                            var file = item;
                            var folderName = Path.Combine("Resources", "WorkFlows", "Results");
                            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                            if (!Directory.Exists(pathToSave))
                            {
                                Directory.CreateDirectory(pathToSave);
                            }
                            if (modelFlow != null)
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
                            obj.Size = Convert.ToDouble(file.Length / 1048576.0);

                            _context.CV_QT_WorkFlowFile.Add(obj);
                        }

                    }
                }

                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0, ms = "Yêu cầu chỉnh sửa công việc thành công!" });

            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1, ms = "Yêu cầu chỉnh sửa công việc không thành công!" });
            }
        }
        #endregion
        #region Nhắc nhở và gia hạn
        //Post: api/MyWorkFlow/r2AddWorkFlowNhacNhoGiaHan
        [HttpPost, DisableRequestSizeLimit]
        [Route("r2AddWorkFlowNhacNhoGiaHan")]
        public async Task<ActionResult<IEnumerable<CV_QT_WorkFlow>>> r2AddWorkFlowNhacNhoGiaHan()
        {
            try
            {
                var modelFlow = JsonConvert.DeserializeObject<FlowModel>(Request.Form["model"]);
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var user = await _context.Sys_Dm_User.FindAsync(userId);
                var cV_QT_WorkFlow = await _context.CV_QT_WorkFlow.FirstOrDefaultAsync(x => x.MyWorkId == modelFlow.MyWorkId && x.TypeFlow == 0); // lọc thông tin công việc đnag được trình duyệt hoàn thành
                if (cV_QT_WorkFlow.Handled != true)
                {
                    cV_QT_WorkFlow.Handled = true;
                    cV_QT_WorkFlow.HandleDate = DateTime.Now;
                }

                var cV_QT_MyWork = await _context.CV_QT_MyWork.FindAsync(modelFlow.MyWorkId);
                if (cV_QT_MyWork != null)
                {


                    string note = "Hệ thống tự động đánh giá chất lượng công việc bị nhắc nhở gia hạn";
                    var error = await _context.CV_DM_Error.FindAsync(1);
                    CV_QT_CounterError cterror = new CV_QT_CounterError();
                    cterror.ErrorId = error.Id;
                    cterror.Point = WorksCommon.countPointAutoNNGh(cV_QT_MyWork.CompleteDate ?? DateTime.Now, cV_QT_MyWork.PointTime ?? 0.0);// tinh diem tu dong
                    cterror.NguoiBiPhatId = cV_QT_MyWork.UserTaskId;
                    cterror.NguoiPhatId = 2028; // Hệ thống
                    cterror.MyWorkId = modelFlow.MyWorkId;
                    cterror.FlowWorkId = cV_QT_WorkFlow.Id;
                    cterror.CreateDate = DateTime.Now;
                    cterror.DepartmentId = user.DepartmentId ?? 0;
                    _context.CV_QT_CounterError.Add(cterror);
                    CV_QT_WorkFlow wflowerror = WorksCommon.objWorkFlow(_context, modelFlow.MyWorkId, 2028, cV_QT_MyWork.UserTaskId, 10, "CV_DANHGIACL", cV_QT_WorkFlow.Id, note, modelFlow.Require, 1);
                    _context.CV_QT_WorkFlow.Add(wflowerror);

                    if (modelFlow.DEnd != null)
                    {
                        cV_QT_MyWork.EndDate = TransforDate.FromDoubleToDate(modelFlow.DEnd ?? 0);
                        await WorksCommon.saveDateChangeMyWork(_context, modelFlow.MyWorkId, cV_QT_MyWork.StartDate ?? DateTime.Now, cV_QT_MyWork.EndDate ?? DateTime.Now, userId);
                    }
                    // luu lai thay doi ve thoi gian ket thuc cong viec
                    CV_QT_WorkFlow wflow2 = WorksCommon.objWorkFlow(_context, modelFlow.MyWorkId, userId, cV_QT_MyWork.UserTaskId, 9, "CV_NHACNHOGIAHAN", cV_QT_WorkFlow.Id, modelFlow.Note, modelFlow.Require, 1); // chuyển hoàn thành công việc
                    _context.CV_QT_WorkFlow.Add(wflow2);
                    await _notifi.SaveNotifiAsync(2, "CV_NHACNHOGIAHAN", wflow2.SendName, "MCV: (" + cV_QT_MyWork.Code.ToString() + ")" + wflow2.Note, wflow2.UserDeliverId, wflow2.TypeFlow, "/giaoviec/quytrinhgiaoviec/congvieccuatoi");
                    if (Request.Form.Files.Count != 0)
                    {
                        foreach (var item in Request.Form.Files)
                        {
                            CV_QT_WorkFlowFile obj = new CV_QT_WorkFlowFile();
                            var file = item;
                            var folderName = Path.Combine("Resources", "WorkFlows", "Results");
                            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                            if (!Directory.Exists(pathToSave))
                            {
                                Directory.CreateDirectory(pathToSave);
                            }
                            if (modelFlow != null)
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
                            obj.WorkFlowId = wflow2.Id;
                            obj.Size = Convert.ToDouble(file.Length / 1048576.0);

                            _context.CV_QT_WorkFlowFile.Add(obj);
                        }
                    }
                }
                // luu quy trình cho người chủ trì

                // lưu quy trình luân chuyển công việc cho người theo dõi
                foreach (var ktem in modelFlow.UserDelivers)
                {
                    CV_QT_WorkFlow wflow = WorksCommon.objWorkFlow(_context, modelFlow.MyWorkId, userId, ktem.UserId, 12, "CV_THEODOI", cV_QT_WorkFlow.Id, modelFlow.Note, modelFlow.Require, 3);
                    // lưu quy trình luân chuyển công việc
                    _context.CV_QT_WorkFlow.Add(wflow);
                    if (Request.Form.Files.Count != 0)
                    {
                        foreach (var item in Request.Form.Files)
                        {
                            CV_QT_WorkFlowFile obj = new CV_QT_WorkFlowFile();
                            var file = item;
                            var folderName = Path.Combine("Resources", "WorkFlows", "Results");
                            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                            if (!Directory.Exists(pathToSave))
                            {
                                Directory.CreateDirectory(pathToSave);
                            }
                            if (modelFlow != null)
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
                            obj.Size = Convert.ToDouble(file.Length / 1048576.0);

                            _context.CV_QT_WorkFlowFile.Add(obj);
                        }

                    }
                }

                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0, ms = "Nhắc nhở và gia hạn công việc thành công!" });

            }
            catch (Exception e)
            {
                return new ObjectResult(new { error = 1, ms = "Nhắc nhở và gia hạn công việc không thành công!" });
            }
        }
        #endregion
        #region Đánh giá chất lượng công việc
        //Post: api/MyWorkFlow/r2AddWorkFlowDGCL
        [HttpPost, DisableRequestSizeLimit]
        [Route("r2AddWorkFlowDGCL")]
        public async Task<ActionResult<IEnumerable<CV_QT_WorkFlow>>> r2AddWorkFlowDGCL()
        {
            try
            {
                var modelFlow = JsonConvert.DeserializeObject<FlowModel>(Request.Form["model"]);
                var userId = Convert.ToInt32(User.Claims.First(c => c.Type == "UserId").Value);
                var user = await _context.Sys_Dm_User.FindAsync(userId);
                List<UserAndStatusDeliver> users = new List<UserAndStatusDeliver>();
                var myWork = await _context.CV_QT_MyWork.FindAsync(modelFlow.MyWorkId); // công việc của tôi
                var preDecWork = await _context.CV_QT_MyWork.FirstOrDefaultAsync(x => x.Code == myWork.Predecessor); // công việc tiên quyết (nếu có)
                foreach (var item in modelFlow.Errors)
                {
                    if (item.Point == null)
                    {
                        var error = await _context.CV_DM_Error.FindAsync(item.Id);
                        if (error != null)
                        {
                            item.Point = error.Point;
                        }
                    }
                    foreach (var utem in modelFlow.TypeUserDelis) // 1: người chủ trì, 2: người phối hợp, 3: người làm trước
                    {
                        var listusers = await WorksCommon.getUserFromTypeUserDeli(_context, utem.Id, modelFlow.MyWorkId); // danh sách người bị phạt
                        if (utem.Id != 3)
                        {

                            foreach (var ttem in listusers)
                            {
                                if (users.Count(x => x.UserId == ttem.UserId) == 0)
                                {
                                    UserAndStatusDeliver userAndStatus = new UserAndStatusDeliver()
                                    {
                                        UserId = ttem.UserId,
                                        Type = utem.Id
                                    };
                                    users.Add(userAndStatus);
                                }
                                CV_QT_CounterError cV_QT_Counter = new CV_QT_CounterError();
                                cV_QT_Counter.MyWorkId = modelFlow.MyWorkId;
                                cV_QT_Counter.FlowWorkId = modelFlow.Id;
                                cV_QT_Counter.CreateDate = DateTime.Now;
                                cV_QT_Counter.DepartmentId = user.DepartmentId ?? 0;
                                cV_QT_Counter.ErrorId = item.Id;
                                cV_QT_Counter.NguoiPhatId = userId;
                                cV_QT_Counter.NguoiBiPhatId = ttem.UserId;
                                cV_QT_Counter.Point = item.Point ?? 0.0;
                                cV_QT_Counter.TypeUserDeli = utem.Id;
                                _context.CV_QT_CounterError.Add(cV_QT_Counter);
                            }
                        }
                        else // chỉ khi có công việc tiên quyết thì mới nhảy vào Type = 3
                        {
                            if (preDecWork != null) // công việc của tôi phải tồn tại công việc tiên quyết thì mới nhảy vào
                            {
                                foreach (var ttem in listusers)
                                {
                                    if (users.Count(x => x.UserId == ttem.UserId) == 0)
                                    {
                                        UserAndStatusDeliver userAndStatus = new UserAndStatusDeliver()
                                        {
                                            UserId = ttem.UserId,
                                            Type = utem.Id
                                        };
                                        users.Add(userAndStatus);
                                    }
                                    CV_QT_CounterError cV_QT_Counter = new CV_QT_CounterError();
                                    cV_QT_Counter.MyWorkId = preDecWork.Id;
                                    cV_QT_Counter.FlowWorkId = modelFlow.Id;
                                    cV_QT_Counter.CreateDate = DateTime.Now;
                                    cV_QT_Counter.DepartmentId = user.DepartmentId ?? 0;
                                    cV_QT_Counter.ErrorId = item.Id;
                                    cV_QT_Counter.NguoiPhatId = userId;
                                    cV_QT_Counter.NguoiBiPhatId = ttem.UserId;
                                    cV_QT_Counter.Point = item.Point ?? 0.0;
                                    cV_QT_Counter.TypeUserDeli = utem.Id;
                                    _context.CV_QT_CounterError.Add(cV_QT_Counter);
                                }
                            }
                        }



                    }
                }
                foreach (var item in users)
                {
                    CV_QT_WorkFlow wflow = new CV_QT_WorkFlow();
                    if (item.Type != 3)
                    {
                        wflow = WorksCommon.objWorkFlow(_context, modelFlow.MyWorkId, userId, item.UserId, 10, "CV_DANHGIACL", modelFlow.Id, modelFlow.Note, modelFlow.Require, 1);
                        // lưu quy trình luân chuyển công việc
                        _context.CV_QT_WorkFlow.Add(wflow);
                    }
                    else
                    {
                        var workPreFlow = _context.CV_QT_WorkFlow.FirstOrDefault(x => x.MyWorkId == preDecWork.Id && x.TypeFlow == 6);
                        if (workPreFlow == null)
                        {
                            workPreFlow = _context.CV_QT_WorkFlow.FirstOrDefault(x => x.MyWorkId == preDecWork.Id && x.TypeFlow == 0); // nếu chưa hoàn thành thì gán giá trị = 0
                            wflow = WorksCommon.objWorkFlow(_context, preDecWork.Id, userId, item.UserId, 10, "CV_DANHGIACL", workPreFlow.Id, modelFlow.Note, modelFlow.Require, 1);
                        }
                        else
                        {
                            wflow = WorksCommon.objWorkFlow(_context, preDecWork.Id, userId, item.UserId, 10, "CV_DANHGIACL", workPreFlow.Id, modelFlow.Note, modelFlow.Require, 1);
                        }

                        // lưu quy trình luân chuyển công việc
                        _context.CV_QT_WorkFlow.Add(wflow);
                    }

                    if (Request.Form.Files.Count != 0)
                    {
                        foreach (var ftem in Request.Form.Files)
                        {
                            CV_QT_WorkFlowFile obj = new CV_QT_WorkFlowFile();
                            var file = ftem;
                            var folderName = Path.Combine("Resources", "WorkFlows", "DanhGiaCL");
                            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                            if (!Directory.Exists(pathToSave))
                            {
                                Directory.CreateDirectory(pathToSave);
                            }
                            if (modelFlow != null)
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

                }
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0, ms = "Đánh giá chất lượng công việc thành công!" });

            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1, ms = "Lỗi đánh giá chất lượng công việc!" });
            }
        }
        #endregion
    }
}