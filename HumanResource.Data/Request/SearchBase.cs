using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Request
{
   public class SearchBase
    {
        public string s { get; set; }
        public int p { get; set; }
        public int pz { get; set; }
        public double? fromDate { get; set; }
        public double? toDate { get; set; }
    }
}
