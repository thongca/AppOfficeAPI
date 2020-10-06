using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Application.Paremeters.Dtos
{
   public class LenhMenuForUser
    {
        public string MenuId { get; set; }
        public int GroupRoleId { get; set; }
    }
    public class LenhMenuForUserOfMyWork
    {
        public string MenuId { get; set; }
        public int GroupRoleId { get; set; }
        public string MyWorkId { get; set; }
    }

    public class BuocLenhGroupForUser
    {
        public string MenuId { get; set; }
        public string MaLenh { get; set; }
        public int BuocLenhGroupId { get; set; }
        public int GroupRoleId { get; set; }
    }
}
