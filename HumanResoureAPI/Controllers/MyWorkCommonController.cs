using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HumanResource.Application.Helper;
using HumanResource.Application.Helper.Dtos;
using HumanResource.Application.Paremeters.Dtos;
using HumanResource.Application.Paremeters.Works;
using HumanResource.Data.EF;
using HumanResource.Data.Entities.VanBan;
using HumanResource.Data.Entities.Works;
using HumanResource.Data.Enum;
using HumanResoureAPI.Common;
using HumanResoureAPI.Common.WorksCommon;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spire.Pdf.Exporting.XPS.Schema;

namespace HumanResoureAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyWorkCommonController : ControllerBase
    {
        private readonly humanDbContext _context;
        public MyWorkCommonController(humanDbContext context)
        {
            _context = context;
        }
        #region Danh sách công việc theo mô tả
        // Get: api/MyWorkCommon/r1GetListWorks
        [HttpGet]
        [Route("r1GetListWorks")]
        public async Task<ActionResult<IEnumerable<CV_DM_DefaultTask>>> r1GetListWorks()
        {
             RequestToken token = CommonData.GetDataFromToken(User);
            
            int DepartmentId =await WorksCommon.getDepartmentID(_context, token.DepartmentId);
            var tables = from a in _context.CV_DM_DefaultTask
                         join b in _context.CV_DM_GroupTask on a.GroupTaskId equals b.Id
                         where a.DepartmentId == DepartmentId
                         select new
                         {
                             a.Name,
                             a.Id,
                             a.LevelTask,
                             a.LevelTime
                         };
            return new ObjectResult(new { error = 0, data = await tables.OrderBy(x => x.Id).ToListAsync() });

        }
        #endregion
        #region Thêm công việc thời xuyên
        // Post: api/MyWorkCommon/r1PostAddWorkDefault
        [HttpPost]
        [Route("r1PostAddWorkDefault")]
        public async Task<ActionResult<IEnumerable<CV_DM_DefaultTask>>> r1PostAddWorkDefault(CV_DM_DefaultTask defaultTask)
        {
            try
            {
                RequestToken token = CommonData.GetDataFromToken(User);
                
                defaultTask.Id = Helper.GenKey();
                defaultTask.Frequency = 1;
                defaultTask.PointTask = getPointTask(defaultTask.LevelTask, 1);
                defaultTask.PointTime = getPointTask(defaultTask.LevelTime, 2);
                defaultTask.DepartmentId = token.DepartmentId;
                _context.CV_DM_DefaultTask.Add(defaultTask);
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0, ms = "Thêm mới công việc thường xuyên thành công!" });
            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1, ms = "Thêm mới công việc thường xuyên không thành công!" });
            }


        }
        #endregion
        #region Danh sách công việc thường xuyên
        // Get: api/MyWorkCommon/r1getListWorkDefault
        [HttpPost]
        [Route("r1getListWorkDefault")]
        public async Task<ActionResult<IEnumerable<CV_DM_DefaultTask>>> r1getListWorkDefault()
        {
            try
            {
                 RequestToken token = CommonData.GetDataFromToken(User);
                
                var defaultTask = await _context.CV_DM_DefaultTask.Where(x => x.DepartmentId == token.DepartmentId).Select(a => new
                {
                    a.Code,
                    a.Id,
                    a.Name,
                    a.LevelTask,
                    a.LevelTime,
                    LevelTaskText = getlevelTask(a.LevelTask, 1),
                    LevelTimeText = getlevelTask(a.LevelTime, 2),
                }).ToListAsync();
                return new ObjectResult(new { error = 0, data = defaultTask });
            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1, ms = "Thêm mới công việc thường xuyên không thành công!" });
            }


        }
        #endregion
        #region Công việc thường xuyên theo ID
        // Get: api/MyWorkCommon/5
        [HttpGet("{Id}")]
        public async Task<ActionResult<IEnumerable<CV_DM_DefaultTask>>> r1getWorkDefaultByID(string Id)
        {
            try
            {
                var defaultWork = await _context.CV_DM_DefaultTask.FindAsync(Id);
                if (defaultWork == null)
                {
                    return NoContent();
                }
                return new ObjectResult(new { error = 0, data = defaultWork });
            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1, ms = "Thêm mới công việc thường xuyên không thành công!" });
            }


        }
        #endregion
        #region Cạp nhật công việc thường xuyên
        // Get: api/MyWorkCommon/5
        [HttpPost]
        public async Task<ActionResult<IEnumerable<CV_DM_DefaultTask>>> r3UpdateWorkDefaultByID(CV_DM_DefaultTask defaultTask)
        {
            try
            {
                var _defaultTask = await _context.CV_DM_DefaultTask.FindAsync(defaultTask.Id);
                if (_defaultTask == null)
                {
                    return new ObjectResult(new { error = 1, ms = "Cập nhật công việc thường xuyên không thành công!" });
                }
                _defaultTask.Name = defaultTask.Name;
                _defaultTask.LevelTask = defaultTask.LevelTask;
                _defaultTask.LevelTime = defaultTask.LevelTime;
                _defaultTask.PointTask = getPointTask(defaultTask.LevelTask, 1);
                _defaultTask.PointTime = getPointTask(defaultTask.LevelTime, 2);
                await _context.SaveChangesAsync();
                return new ObjectResult(new { error = 0, ms = "Cập nhật công việc thường xuyên thành công!" });
            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1, ms = "Cập nhật công việc thường xuyên không thành công!" });
            }


        }
        #endregion
        #region Xóa dm công việc thường xuyên
        // Post: api/MyWorkCommon/r4DelCV_DM_DefaultTask
        [HttpPost]
        [Route("r4DelCV_DM_DefaultTask")]
        public async Task<IActionResult> r4DelCV_DM_DefaultTask(List<CV_DM_DefaultTask> listDataRms)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { error = 1 });
            }
            _context.CV_DM_DefaultTask.RemoveRange(listDataRms);
            await _context.SaveChangesAsync();
            return new JsonResult(new { error = 0 });

        }
        #endregion
        #region Danh sách nhoms công việc theo mô tả
        // Get: api/MyWorkCommon/r1GetListGroupWork
        [HttpGet]
        [Route("r1GetListGroupWork")]
        public async Task<ActionResult<IEnumerable<CV_DM_DefaultTask>>> r1GetListGroupWorks()
        {
             RequestToken token = CommonData.GetDataFromToken(User);
            
            var tables = from a in _context.CV_DM_GroupTask
                         where a.DepartmentId == token.DepartmentId
                         select new
                         {
                             a.Name,
                             a.Id,
                         };
            return new ObjectResult(new { error = 0, data = await tables.OrderBy(x => x.Id).ToListAsync() });

        }
        #endregion
        #region Danh sách lịch sử thực hiện công việc
        // Get: api/MyWorkCommon/r1GetListHistory
        [HttpPost]
        [Route("r1GetListHistory")]
        public async Task<ActionResult<IEnumerable<CV_QT_StartPauseHistory>>> r1GetListHistory(OptionsCv option)
        {
             RequestToken token = CommonData.GetDataFromToken(User);
            var tables = from a in _context.CV_QT_StartPauseHistory
                         join b in _context.CV_QT_MyWork on a.MyWorkId equals b.Id
                         where a.UserCreateId == token.UserID && a.MyWorkId == option.MyWorkId
                         select new
                         {
                             b.TaskCode,
                             b.TaskName,
                             a.CreateDate,
                             a.CycleWork,
                             a.Id,
                         };
            return new ObjectResult(new { error = 0, data = await tables.OrderBy(x => x.Id).ToListAsync() });

        }
        #endregion
        #region Danh sách lỗi trong form đánh giá chất lượng 
        // Get: api/MyWorkCommon/r1GetListErrorCTG
        [HttpGet]
        [Route("r1GetListErrorCTG")]
        public async Task<ActionResult<IEnumerable<CV_QT_StartPauseHistory>>> r1GetListErrorCTG()
        {
             RequestToken token = CommonData.GetDataFromToken(User);
            
            var room = await _context.Sys_Dm_Department.FindAsync(token.DepartmentId);
            int DepId = 0;
            if (room.ParentId == null)
            {
                DepId = room.Id;
            }
            else
            {
                DepId = room.ParentId ?? 0;
            }
            var tables = from a in _context.CV_DM_Error
                         where a.DepartmentId == DepId && a.Active != true
                         select new
                         {
                             a.ErrorName,
                             a.Point,
                             a.Id,
                         };
            return new ObjectResult(new { error = 0, data = await tables.OrderBy(x => x.Id).ToListAsync(), total = tables.Count() });

        }
        #endregion
        #region Danh sách lỗi trong form hiệu quả công việc
        // Get: api/MyWorkCommon/r1GetListErrorhqcv
        [HttpGet]
        [Route("r1GetListErrorhqcv")]
        public async Task<ActionResult<IEnumerable<CV_QT_StartPauseHistory>>> r1GetListErrorhqcv()
        {
             RequestToken token = CommonData.GetDataFromToken(User);
            
            var room = await _context.Sys_Dm_Department.FindAsync(token.DepartmentId);
            int DepId = 0;
            if (room.ParentId == null)
            {
                DepId = room.Id;
            }
            else
            {
                DepId = room.ParentId ?? 0;
            }
            var tables = from a in _context.CV_DM_Error
                         where a.DepartmentId == DepId
                         select new
                         {
                             a.ErrorName,
                             a.Point,
                             a.Id,
                         };
            return new ObjectResult(new { error = 0, data = await tables.OrderBy(x => x.Id).ToListAsync(), total = tables.Count() });

        }
        #endregion
        #region Danh sách lệnh chọn
        // Post: api/MyWorkCommon/r1GetListDataLenhTheoUser
        [HttpPost]
        [Route("r1GetListDataLenhTheoUser")]
        public async Task<ActionResult<IEnumerable<VB_QT_Buoc>>> r1GetListDataLenhTheoUser(LenhMenuForUserOfMyWork options)
        {
            try
            {
                 RequestToken token = CommonData.GetDataFromToken(User);
                var workFlows = _context.CV_QT_WorkFlow.Where(x => x.MyWorkId == options.MyWorkId).Select(x => x.TypeFlow).Distinct().ToList();
                var myWork = await _context.CV_QT_MyWork.FindAsync(options.MyWorkId);
                List<string> list = new List<string>();
                if (myWork != null)
                {
                    if (myWork.CycleWork == 0)
                    {
                        list.Add("CV_TRINHHOANTHANH");
                    } else
                    {
                        list.Add("CV_ASSIGNWORK");
                    }
                }
                if (!workFlows.Contains(TypeFlowEnum.TrinhPheDuyetThoiHan) && !workFlows.Contains(TypeFlowEnum.CongViecKhoiTaoSau))
                {
                    list.Add("CV_TRINHHOANTHANH");
                    list.Add("CV_TRINHCHINHSUA");
                }
                if (workFlows.Contains(TypeFlowEnum.TrinhPheDuyetThoiHan))
                {
                    list.Add("CV_TRINHTHOIHAN");
                    list.Add("CV_TRINHCHINHSUA");
                }
                if (workFlows.Contains(TypeFlowEnum.DaPheDuyetThoiHanCoChinhSua) || workFlows.Contains(TypeFlowEnum.DaPheDuyetThoiHanKhongChinhSua))
                {
                    list.Add("CV_TRINHTHOIHAN");
                    list.Add("CV_DUYETTHOIHAN");
                }
                if (workFlows.Contains(TypeFlowEnum.TrinhPheDuyetKetQua) && !workFlows.Contains(TypeFlowEnum.DaPheDuyetKetQuaYeuCauChinhSua))
                {
                    list.Add("CV_TRINHHOANTHANH");

                }
                if (workFlows.Contains(TypeFlowEnum.DaPheDuyetKetQuaDatChatLuong))
                {
                    list.Add("CV_TRINHHOANTHANH");
                    list.Add("CV_DUYETHOANTHANH");
                }
                if (workFlows.Contains(TypeFlowEnum.CongViecKhoiTaoSau))
                {
                    list.Add("CV_TRINHTHOIHAN");
                    list.Add("CV_TRINHCHINHSUA");
                }
                
                if(workFlows.Contains(TypeFlowEnum.TrinhHoanThanhKhoiTaoSau) && workFlows.Contains(TypeFlowEnum.DuyetCongViecKhoiTaoSau))
                {
                    list.Add("CV_KHOITAOSAU");
                    list.Add("CV_DUYETKHOITAOSAU"); 
                }

                var tables = from a in _context.VB_QT_BuocLenhGroupRole
                             where a != null
                             join b in _context.VB_QT_BuocLenhTuongTac on a.BuocLenhTuongTacId equals b.Id
                             join c in _context.VB_QT_LenhTuongTac on b.LenhTuongTacId equals c.Id
                             join d in _context.VB_QT_Buoc on b.BuocId equals d.Id
                             where a.GroupRoleId == options.GroupRoleId && d.MenuId == options.MenuId && !list.Contains(c.Code)
                             select new
                             {
                                 c.Name,
                                 BuocLenhGroupId = a.Id,
                                 c.IsActive,
                                 c.IsOrder,
                                 c.Code

                             };
                var qrs = await tables.OrderBy(x => x.IsOrder).ToListAsync();
                return new ObjectResult(new { error = 0, data = qrs });
            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }
        }
        #endregion
        private static string getlevelTask(int level, int type)
        {
            // type = 1 là độ khó , = 2 là độ vội
            if (type == 1)
            {
                switch (level)
                {
                    case 1:
                        return "Dễ";
                    case 2:
                        return "Trung bình";
                    case 3:
                        return "Khó";
                    case 4:
                        return "Rất khó";
                    default:
                        return "Dễ";
                }
            }
            else
            {
                switch (level)
                {
                    case 1:
                        return "Bình thường";
                    case 2:
                        return "Vội";
                    case 3:
                        return "Rất vội";
                    default:
                        return "Bình thường";
                }
            }

        }
        private static double getPointTask(int level, int type)
        {
            // type = 1 là độ khó , = 2 là độ vội
            if (type == 1)
            {
                switch (level)
                {
                    case 1:
                        return 0.9;
                    case 2:
                        return 1.0;
                    case 3:
                        return 1.1;
                    case 4:
                        return 1.2;
                    default:
                        return 0.9;
                }
            }
            else
            {
                switch (level)
                {
                    case 1:
                        return 1.0;
                    case 2:
                        return 1.1;
                    case 3:
                        return 1.2;
                    default:
                        return 1.0;
                }
            }

        }
    }
}