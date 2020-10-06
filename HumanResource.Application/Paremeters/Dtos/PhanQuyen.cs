using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Application.Paremeters.Dtos
{
    public class PhanQuyen
    {
        public int Id { get; set; }
        public List<MenuPhanQuyen> menuPhanQuyens { get; set; }
    }
    public class MenuPhanQuyen
    {
        public string Id { get; set; }
        public bool AddPer { get; set; }
        public bool ViewPer { get; set; }
        public bool EditPer { get; set; }
        public bool DelPer { get; set; }
        public bool ExportPer { get; set; }
    }
    public class GroupRole
    {
        public int Id { get; set; }
    }
}
