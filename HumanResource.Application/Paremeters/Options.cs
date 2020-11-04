using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Application.Paremeters
{
    public class Options
    {
        public string s { get; set; }
        public int p { get; set; }
        public int pz { get; set; }
        public int companyId { get; set; }
        public int departmentId { get; set; }
        public int nestId { get; set; }
        public int groupId { get; set; }
        public int? rankrole { get; set; }
    }
    public class OptionNull
    {
        public int companyId { get; set; }
        public int departmentId { get; set; }
        public int nestId { get; set; }
        public int groupRoleId { get; set; }
    }

    public class OptionSelectUser
    {
        public int companyId { get; set; }
        public int departmentId { get; set; }
        public int nestId { get; set; }
        public int groupRoleId { get; set; }
        public string s { get; set; }
    }

    public class OptionThongBao
    {
       public int NguoiNhanId { get; set; }
    }
    public class NguoiNhanXNHT
    {
        public int Id { get; set; }
    }
    public class OptionId
    {
        public int Id { get; set; }
    }
    public class OptionRePort
    {
        public int UserId { get; set; }
    }
}
