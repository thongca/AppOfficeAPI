using HumanResource.Data.EF;
using HumanResource.Data.Enum;
using HumanResource.Data.Request;
using HumanResoureAPI.Common;
using HumanResoureAPI.HelperPara;
using HumanResoureAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumanResoureAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LevelTimeController : ControllerBase
    {
        private readonly CV_DM_LevelTimeService _service;

        public LevelTimeController(humanDbContext context)
        {
            _service = new CV_DM_LevelTimeService(context); ;
        }
        // Post: api/CV_DM_LevelTime/r1GetListData
        [HttpPost]
        [Route("r1GetListData")]
        public ActionResult r1GetListData(CV_DM_LevelTimeRequest search)
        {
            try
            {
                RequestToken token = CommonData.GetDataFromToken(User);
                var response = _service.GetItems(search, token);
                return new ObjectResult(new
                {
                    error = response.Code,
                    data = response.Data,
                    total = response.Total,
                    ms = PageHelper.GetEnumDescription(response.Code, "Lấy dữ liệu mức độ ưu tiên thành công!")
                });
            }
            catch (Exception)
            {
                return new ObjectResult(new { error = 1 });
            }
        }

        // GET: api/CV_DM_LevelTime/5
        [HttpGet("r1GetItemById/{id}")]
        public ActionResult GetItemById(int id)
        {
            try
            {
                RequestToken token = CommonData.GetDataFromToken(User);
                var result = _service.GetItem(id, token);
                return new ObjectResult(new { error = ErrorCodeEnum.Success, data = result, ms = PageHelper.GetEnumDescription(ErrorCodeEnum.Success, "Lấy dữ liệu mức độ ưu tiên thành công!") });
            }
            catch (Exception)
            {
                return new ObjectResult(new { error = ErrorCodeEnum.Exception, ms = PageHelper.GetEnumDescription(ErrorCodeEnum.Exception) });
            }
        }

        // PUT: api/CV_DM_LevelTime/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost("r3_UpdateData")]
        public IActionResult r3_EditData(CV_DM_LevelTimeRequest request)
        {
            try
            {
                RequestToken token = CommonData.GetDataFromToken(User);
                var result = _service.EditItem(request, token);
                return new ObjectResult(new { error = result, ms = PageHelper.GetEnumDescription(result, "Cập nhật dữ liệu mức độ ưu tiên thành công!") });
            }
            catch (DbUpdateConcurrencyException)
            {
                return new ObjectResult(new { error = ErrorCodeEnum.Exception, ms = PageHelper.GetEnumDescription(ErrorCodeEnum.Exception) });
            }
        }

        // POST: api/CV_DM_LevelTime
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost("r2_CreateData")]
        public ActionResult r2_AddData(CV_DM_LevelTimeRequest request)
        {
            try
            {
                RequestToken token = CommonData.GetDataFromToken(User);
                var result = _service.CreateItem(request, token);
                return new ObjectResult(new { error = result, ms = PageHelper.GetEnumDescription(result, "Tạo dữ liệu mức độ ưu tiên thành công!") });
            }
            catch (Exception)
            {
                return new ObjectResult(new { error = ErrorCodeEnum.Exception, ms = PageHelper.GetEnumDescription(ErrorCodeEnum.Exception) });
            }
        }

        [HttpPost]
        [Route("r4_DeleteData")]
        public IActionResult r4_DeleteData([FromBody] List<CV_DM_LevelTimeRequest> requests)
        {
            try
            {
                var Ids = requests.Select(a => a.Id).ToList();
                RequestToken token = CommonData.GetDataFromToken(User);
                var result = _service.DeletedItems(Ids, token);
                return new ObjectResult(new { error = result, ms = PageHelper.GetEnumDescription(result, "Xóa dữ liệu mức độ ưu tiên thành công!") });
            }
            catch (Exception)
            {
                return new ObjectResult(new { error = ErrorCodeEnum.Exception, ms = PageHelper.GetEnumDescription(ErrorCodeEnum.Exception) });
            }
        }
    }
}
