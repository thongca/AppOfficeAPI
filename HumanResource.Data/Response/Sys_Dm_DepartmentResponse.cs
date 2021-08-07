using HumanResource.Data.Entities.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Response
{
   public class Sys_Dm_DepartmentResponse
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public int CompanyId { get; set; }
        public int IsOrder { get; set; }
        public bool IsActive { get; set; }
        public Sys_Dm_DepartmentResponse()
        {

        }
        public Sys_Dm_DepartmentResponse(Sys_Dm_Department model)
        {
            Id = model.Id;
            Code = model.Code;
            Name = model.Name;
            ParentId = model.ParentId;
        }
    }
}
