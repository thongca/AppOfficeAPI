using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Application.Paremeters
{
   public class CheckLogin
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class ChangePass
    {
        public string PassOld { get; set; }
        public string PassNew { get; set; }
    }
}
