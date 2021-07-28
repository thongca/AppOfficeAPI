using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HumanResource.Application.Helper.Dtos;
using HumanResource.Data.DTO;
using HumanResource.Data.EF;
using HumanResource.Data.Entities.Works;
using HumanResource.Data.Enum;
using HumanResoureAPI.Common;
using HumanResoureAPI.Common.WorksCommon;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HumanResoureAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PheDuyetMyWorkController : ControllerBase
    {
        private readonly humanDbContext _context;
        public PheDuyetMyWorkController(humanDbContext context)
        {
            _context = context;
        }
        #region Phê duyệt thời gian
        //Post: api/MyWork/r2AddFLowDeadline
        [HttpPost, DisableRequestSizeLimit]
        [Route("r2AddDuyetThoiHan")]
        public async Task<ActionResult<IEnumerable<CV_QT_MyWork>>> r2AddFLowDeadline()
        {
            try
            {
                var model = JsonConvert.DeserializeObject<Dtos_FlowWorkPheDuyetTH>(Request.Form["model"]);
                 RequestToken token = CommonData.GetDataFromToken(User);
                // lưu quy trình luân chuyển công việc phê duyệt công việc
                CV_QT_WorkFlow wflow = WorksCommon.objWorkFlow(_context, model.CV_QT_WorkFlow.MyWorkId, token.UserID, model.CV_QT_WorkFlow.UserDeliverId, model.CV_QT_WorkFlow.TypeFlow, "CV_MYWORK", model.CV_QT_WorkFlow.ParentId, model.CV_QT_WorkFlow.Note, model.CV_QT_WorkFlow.Require, 1);
                if (model.CV_QT_WorkFlow.TypeFlow == TypeFlowEnum.DaPheDuyetThoiHanCoChinhSua)
                {
                    var myWork =await _context.CV_QT_MyWork.FindAsync(model.CV_QT_WorkFlow.MyWorkId);
                    myWork.EndDate = model.ChangeDate;
                }
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
                        if (model != null)
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
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        #region Phê duyệt đạt chất lượng
        //Post: api/MyWork/r2AddFLowDeadline
        [HttpPost, DisableRequestSizeLimit]
        [Route("r2AddDuyetDatChatLuong")]
        public async Task<ActionResult<IEnumerable<CV_QT_MyWork>>> r2AddDuyetDatChatLuong()
        {
            try
            {
                var model = JsonConvert.DeserializeObject<Dtos_FlowWork>(Request.Form["model"]);
                 RequestToken token = CommonData.GetDataFromToken(User);
                // lưu quy trình luân chuyển công việc phê duyệt công việc
                CV_QT_WorkFlow wflow = WorksCommon.objWorkFlow(_context, model.CV_QT_WorkFlow.MyWorkId, token.UserID, model.CV_QT_WorkFlow.UserDeliverId, TypeFlowEnum.DaPheDuyetKetQuaDatChatLuong, "CV_HOANTHANH", model.CV_QT_WorkFlow.ParentId, model.CV_QT_WorkFlow.Note, model.CV_QT_WorkFlow.Require, 1);
                _context.CV_QT_WorkFlow.Add(wflow);
                List<CV_QT_WorkFlowFile> _WorkFlowFiles = new List<CV_QT_WorkFlowFile>();
                if (Request.Form.Files.Count != 0)
                {
                    foreach (var item in Request.Form.Files)
                    {
                        CV_QT_WorkFlowFile obj = new CV_QT_WorkFlowFile();
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
                        _WorkFlowFiles.Add(obj);
                        _context.CV_QT_WorkFlowFile.Add(obj);
                    }

                }
                // cc cho người cần thông tin
                foreach (var item in model.CV_QT_CCUsers)
                {
                    CV_QT_WorkFlow wflowcc = new CV_QT_WorkFlow();
                     wflowcc = WorksCommon.objWorkFlow(_context, model.CV_QT_WorkFlow.MyWorkId, token.UserID, item.UserId, TypeFlowEnum.DaPheDuyetKetQuaDatChatLuong, "CV_HOANTHANH", model.CV_QT_WorkFlow.ParentId, model.CV_QT_WorkFlow.Note, model.CV_QT_WorkFlow.Require, 3);
                    _context.CV_QT_WorkFlow.Add(wflowcc);
                    foreach (var ftem in _WorkFlowFiles)
                    {
                        CV_QT_WorkFlowFile obj2 = new CV_QT_WorkFlowFile();
                        obj2.Extension = ftem.Extension;
                        obj2.Path = ftem.Path;
                        obj2.Name = ftem.Name;
                        obj2.WorkFlowId = wflowcc.Id;
                        obj2.Size = ftem.Size;
                        _context.CV_QT_WorkFlowFile.Add(obj2);
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
        #region Yêu cầu chỉnh sửa
        //Post: api/MyWork/r2AddDuyetYeuCauChinhSua
        [HttpPost, DisableRequestSizeLimit]
        [Route("r2AddDuyetYeuCauChinhSua")]
        public async Task<ActionResult<IEnumerable<CV_QT_MyWork>>> r2AddDuyetYeuCauChinhSua()
        {
            try
            {
                var model = JsonConvert.DeserializeObject<Dtos_FlowWork>(Request.Form["model"]);
                 RequestToken token = CommonData.GetDataFromToken(User);
                // lưu quy trình luân chuyển công việc phê duyệt công việc
                CV_QT_WorkFlow wflow = WorksCommon.objWorkFlow(_context, model.CV_QT_WorkFlow.MyWorkId, token.UserID, model.CV_QT_WorkFlow.UserDeliverId, TypeFlowEnum.DaPheDuyetKetQuaYeuCauChinhSua, "CV_MYWORK", model.CV_QT_WorkFlow.ParentId, model.CV_QT_WorkFlow.Note, model.CV_QT_WorkFlow.Require, 1);
                _context.CV_QT_WorkFlow.Add(wflow);
                List<CV_QT_WorkFlowFile> _WorkFlowFiles = new List<CV_QT_WorkFlowFile>();
                if (Request.Form.Files.Count != 0)
                {
                    foreach (var item in Request.Form.Files)
                    {
                        CV_QT_WorkFlowFile obj = new CV_QT_WorkFlowFile();
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
                        _WorkFlowFiles.Add(obj);
                        _context.CV_QT_WorkFlowFile.Add(obj);
                    }

                }
                // cc thông tin cho người cần biết người dùng chọn từ ngoài hệ thống 
                foreach (var item in model.CV_QT_CCUsers)
                {
                    CV_QT_WorkFlow wflowcc = new CV_QT_WorkFlow();
                    wflowcc = WorksCommon.objWorkFlow(_context, model.CV_QT_WorkFlow.MyWorkId, token.UserID, item.UserId, TypeFlowEnum.DaPheDuyetKetQuaYeuCauChinhSua, "CV_MYWORK", model.CV_QT_WorkFlow.ParentId, model.CV_QT_WorkFlow.Note, model.CV_QT_WorkFlow.Require, 3);
                    _context.CV_QT_WorkFlow.Add(wflowcc);
                    foreach (var ftem in _WorkFlowFiles)
                    {
                        CV_QT_WorkFlowFile obj2 = new CV_QT_WorkFlowFile();
                        obj2.Extension = ftem.Extension;
                        obj2.Path = ftem.Path;
                        obj2.Name = ftem.Name;
                        obj2.WorkFlowId = wflowcc.Id;
                        obj2.Size = ftem.Size;
                        _context.CV_QT_WorkFlowFile.Add(obj2);
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