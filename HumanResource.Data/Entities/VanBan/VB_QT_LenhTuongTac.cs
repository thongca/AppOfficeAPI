﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.VanBan
{
   public class VB_QT_LenhTuongTac
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public int IsOrder { get; set; }
        public bool IsActive { get; set; }
        public string QuyTrinhId { get; set; }
        public int CompanyId { get; set; }
    }
}
