using HumanResource.Data.EF;
using HumanResource.Data.Entities.Works;
using HumanResource.Data.Enum;
using HumanResource.Data.Interface;
using HumanResource.Data.Request;
using HumanResource.Data.Response;
using HumanResoureAPI.HelperPara;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumanResoureAPI.Services
{
    public class CV_DM_LevelTimeService : IBaseService<CV_DM_LevelTime, CV_DM_LevelTimeRequest, CV_DM_LevelTimeResponse>
    {
        private readonly humanDbContext _context;
        public CV_DM_LevelTimeService(humanDbContext context)
        {
            _context = context;
        }
        public bool CheckExistCode(CV_DM_LevelTime request)
        {
            throw new NotImplementedException();
        }

        public ErrorCodeEnum CreateItem(CV_DM_LevelTimeRequest request, RequestToken token)
        {
            CV_DM_LevelTime obj = new CV_DM_LevelTime(request);
            obj = DataDefault(obj, token);
            _context.CV_DM_LevelTime.Add(obj);
            _context.SaveChanges();
            return ErrorCodeEnum.Success;
        }

        public CV_DM_LevelTime DataDefault(CV_DM_LevelTime data, RequestToken token)
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
            var data = _context.CV_DM_LevelTime.Find(Id);
            data.Deleted = true;
            _context.SaveChanges();
            return ErrorCodeEnum.Success;
        }

        public ErrorCodeEnum DeletedItems(List<int> Ids, RequestToken token)
        {

            foreach (var item in Ids)
            {
                var data = _context.CV_DM_LevelTime.Find(item);
                data.Deleted = true;
            }
            _context.SaveChanges();
            return ErrorCodeEnum.Success;
        }

        public ErrorCodeEnum EditItem(CV_DM_LevelTimeRequest request, RequestToken token)
        {
            if (request.Id == null || request.Id == 0)
            {
                return ErrorCodeEnum.PutError;
            }
            var data = _context.CV_DM_LevelTime.Find(request.Id);
            data.Name = request.Name;
            data.Point = request.Point;
            _context.SaveChanges();
            return ErrorCodeEnum.Success;
        }

        public CV_DM_LevelTimeResponse GetItem(int Id, RequestToken token)
        {
            var data = _context.CV_DM_LevelTime.Find(Id);
            return new CV_DM_LevelTimeResponse(data);
        }

        public ResponseBase<CV_DM_LevelTimeResponse> GetItems(CV_DM_LevelTimeRequest request, RequestToken token)
        {

            var data = _context.CV_DM_LevelTime.Where(x => x.Deleted != true && x.CompanyId == token.CompanyId).Select(a => new CV_DM_LevelTimeResponse(a));
            var result = PageHelper.GetPage(data, request);
            return result;
        }
    }
}
