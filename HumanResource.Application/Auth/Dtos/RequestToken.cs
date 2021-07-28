using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Application.Helper.Dtos
{
   public class RequestToken
    {
        public int UserID { get; set; }
        public int CompanyId { get; set; }
        public int GroupRoleId { get; set; }
    }
}
