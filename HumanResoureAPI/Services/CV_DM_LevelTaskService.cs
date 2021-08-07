using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HumanResource.Data.EF;
using HumanResource.Data.Entities.Works;
using HumanResource.Data.Enum;
using HumanResource.Data.Interface;
using HumanResource.Data.Request;
using HumanResource.Data.Response;
using HumanResoureAPI.HelperPara;

namespace HumanResoureAPI.Services
{
    public class CV_DM_LevelTaskService : IBaseService<CV_DM_LevelTask, CV_DM_LevelTaskRequest, CV_DM_LevelTaskResponse>
    {
        private readonly humanDbContext _context;
        public CV_DM_LevelTaskService(humanDbContext context)
        {
            _context = context;
        }
        public bool CheckExistCode(CV_DM_LevelTask request)
        {
            throw new NotImplementedException();
        }

        public ErrorCodeEnum CreateItem(CV_DM_LevelTaskRequest request, RequestToken token)
        {
            CV_DM_LevelTask obj = new CV_DM_LevelTask(request);
            obj = DataDefault(obj, token);
            _context.CV_DM_LevelTask.Add(obj);
            _context.SaveChanges();
            return ErrorCodeEnum.Success;
        }

        public CV_DM_LevelTask DataDefault(CV_DM_LevelTask data, RequestToken token)
        {
            data.CompanyId = token.CompanyId;
            data.DepartmentId = token.DepartmentId;
            data.Deleted = false;
            return data;
        }

        public ErrorCodeEnum DeletedItem(int? Id, RequestToken token)
        {
            if (Id == 0 && Id == null)
            {
                return ErrorCodeEnum.DeleteError;
            }
            var data = _context.CV_DM_LevelTask.Find(Id);
            data.Deleted = true;
            _context.SaveChanges();
            return ErrorCodeEnum.Success;
        }

        public ErrorCodeEnum DeletedItems(List<int> Ids, RequestToken token)
        {
           
            foreach (var item in Ids)
            {
                var data = _context.CV_DM_LevelTask.Find(item);
                data.Deleted = true;
            }
            _context.SaveChanges();
            return ErrorCodeEnum.Success;
        }

        public ErrorCodeEnum EditItem(CV_DM_LevelTaskRequest request, RequestToken token)
        {
            if (request.Id == null || request.Id == 0)
            {
                return ErrorCodeEnum.PutError;
            }
            var data = _context.CV_DM_LevelTask.Find(request.Id);
            data.Name = request.Name;
            data.Point = request.Point;
            _context.SaveChanges();
            return ErrorCodeEnum.Success;
        }

        public CV_DM_LevelTaskResponse GetItem(int Id, RequestToken token)
        {
            var data = _context.CV_DM_LevelTask.Find(Id);
            return new CV_DM_LevelTaskResponse(data);
        }

        public ResponseBase<CV_DM_LevelTaskResponse> GetItems(CV_DM_LevelTaskRequest request, RequestToken token)
        {

            var data = _context.CV_DM_LevelTask.Where(x => x.Deleted != true && x.CompanyId == token.CompanyId).Select(a => new CV_DM_LevelTaskResponse(a));
            var result = PageHelper.GetPage(data, request);
            return result;
        }
    }
}
