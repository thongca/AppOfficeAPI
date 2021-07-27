using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HumanResource.Data.Enum
{
   public enum RoleUserEnum
    {
        [Description("Quản trị viên tổng")]
        Administrator = 0,
        [Description("Quản trị viên công ty")]
        AdminCompany = 1,
        [Description("Quản trị viên chi nhánh")]
        AdminBranch = 2,
        [Description("Quản trị viên phòng")]
        AdminDepartment = 3,
        [Description("Quản trị viên tổ")]
        AdminNest = 4,
        [Description("Quyền thường")]
        Normal = 5
    }
}
