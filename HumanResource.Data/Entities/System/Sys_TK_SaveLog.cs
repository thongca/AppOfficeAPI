using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.System
{
   public class Sys_TK_SaveLog
    {
        public int Id { get; set; }
        public string UrlApi { get; set; }
        public string ErrorLog { get; set; }
        public string Description { get; set; }
    }
}
