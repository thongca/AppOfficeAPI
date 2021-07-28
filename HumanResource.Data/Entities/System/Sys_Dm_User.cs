using HumanResource.Data.Enum;
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
        /// <summary>
        /// Phòng ban
        /// </summary>
        public int? DepartmentId { get; set; }
        /// <summary>
        /// Chức vụ
        /// </summary>
        public int? PositionId { get; set; }
        public string PositionName { get; set; }
        public string DepartmentName { get; set; }
        /// <summary>
        /// Phòng ban cha
        /// </summary>
        public int? ParentDepartId { get; set; }
        public string Email { get; set; }
        public Nullable<int> NestId { get; set; }
        public RoleUserEnum Role { get; set; }
        public Nullable<int>  GroupRoleId { get; set; }
    }
}
