using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Application.Paremeters.Dtos
{
   public class MoHinhToChuc
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Loai { get; set; }

        public int IsOrder { get; set; }
        public int CompanyId { get; set; }
        public List<MoHinhToChuc> children { get; set; }
    }
}
