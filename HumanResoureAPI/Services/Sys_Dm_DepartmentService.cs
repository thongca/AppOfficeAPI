using HumanResource.Data.EF;
using HumanResource.Data.Entities.System;
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
    public class Sys_Dm_DepartmentService : IBaseService<Sys_Dm_Department, Sys_Dm_DepartmentRequest, Sys_Dm_DepartmentResponse>
    {
        private readonly humanDbContext _context;
        public Sys_Dm_DepartmentService(humanDbContext context)
        {
            _context = context;
        }
        public bool CheckExistCode(Sys_Dm_Department request)
        {
            var code_exist = _context.Sys_Dm_Department.Count(x => x.Code == request.Code);
            if (code_exist > 0)
            {
                return true;
            }
            return false;
        }

        public ErrorCodeEnum CreateItem(Sys_Dm_DepartmentRequest request, RequestToken token)
        {
            var code_exist = _context.Sys_Dm_Department.Count(x => x.Code == request.Code);
            if (code_exist > 0)
            {
                return ErrorCodeEnum.CodeExist;
            }
            Sys_Dm_Department obj = new Sys_Dm_Department(request);
            obj = DataDefault(obj, token);
            _context.Sys_Dm_Department.Add(obj);
            _context.SaveChanges();
            var department = _context.Sys_Dm_Department.FirstOrDefault(x => x.Code == obj.Code);
            CloneDmError(department.Id, token);
            return ErrorCodeEnum.Success;
        }
        private ErrorCodeEnum CloneDmError(int DepartmentId, RequestToken token)
        {
            var errors = _context.CV_DM_Error.Where(x => x.CompanyId == 3 && x.Active == true);
            foreach (var item in errors)
            {
                item.Id = 0;
                item.DepartmentId = DepartmentId;
                item.CompanyId = token.CompanyId;
                item.Deleted = false;
                _context.CV_DM_Error.Add(item);
            }
            _context.SaveChanges();
            return ErrorCodeEnum.Success;
        }
        public Sys_Dm_Department DataDefault(Sys_Dm_Department data, RequestToken token)
        {
            data.CompanyId = token.CompanyId;
            return data;
        }

        public ErrorCodeEnum DeletedItem(int? Id, RequestToken token)
        {
            if (Id == 0 && Id == null)
            {
                return ErrorCodeEnum.DeleteError;
            }
            var data = _context.Sys_Dm_Department.Find(Id);
            _context.SaveChanges();
            return ErrorCodeEnum.Success;
        }

        public ErrorCodeEnum DeletedItems(List<int> Ids, RequestToken token)
        {

            foreach (var item in Ids)
            {
                var data = _context.Sys_Dm_Department.Find(item);
                _context.Sys_Dm_Department.Remove(data);
            }
            _context.SaveChanges();
            return ErrorCodeEnum.Success;
        }

        public ErrorCodeEnum EditItem(Sys_Dm_DepartmentRequest request, RequestToken token)
        {
            if (request.Id == 0)
            {
                return ErrorCodeEnum.PutError;
            }
            var data = _context.Sys_Dm_Department.Find(request.Id);
            data.Name = request.Name;
            _context.SaveChanges();
            return ErrorCodeEnum.Success;
        }

        public Sys_Dm_DepartmentResponse GetItem(int Id, RequestToken token)
        {
            var data = _context.Sys_Dm_Department.Find(Id);
            return new Sys_Dm_DepartmentResponse(data);
        }

        public ResponseBase<Sys_Dm_DepartmentResponse> GetItems(Sys_Dm_DepartmentRequest request, RequestToken token)
        {

            var data = _context.Sys_Dm_Department.Where(x => x.CompanyId == token.CompanyId).Select(a => new Sys_Dm_DepartmentResponse(a));
            var result = PageHelper.GetPage(data, request);
            return result;
        }
    }
}
