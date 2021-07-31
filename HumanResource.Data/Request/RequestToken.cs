using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Request
{
   public class RequestToken
    {
        public int UserID { get; set; }
        /// <summary>
        /// Tên người dùng đang đăng nhập
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// Phòng ban người dùng đang đăng nhập
        /// </summary>
        public int DepartmentId { get; set; }
        public int CompanyId { get; set; }
        public int GroupRoleId { get; set; }
    }
}
