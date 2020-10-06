using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.System
{
   public class Sys_Dm_User
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public int? CompanyId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? UserCreateId { get; set; }
        public int? DepartmentId { get; set; }
        public int? PositionId { get; set; }
        public string PositionName { get; set; }
        public string DepartmentName { get; set; }
        public int? ParentDepartId { get; set; }
        public string Email { get; set; }
    }
}
