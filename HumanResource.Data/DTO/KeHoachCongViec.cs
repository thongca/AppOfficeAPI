using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.DTO
{
   public class KeHoachCongViec
    {
        public string ResourceNames { get; set; }
        public int NgayBD { get; set; }
        public int KeoDai { get; set; }
        public string NameCV { get; set; }
        public int Status { get; set; }
    }
    public class CongViecNhanhModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public string Code { get; set; }
        public int TotalDay { get; set; }
        public string MieuTa { get; set; }
        public string CongViecMoiId { get; set; }
        public string LuanChuyenCVId { get; set; }
        public string GroupCVId { get; set; }
        public string TenNhom { get; set; }
        public string LinhVucCvId { get; set; }
        public int LevelCv { get; set; }
        public List<NguoiNhan> nguoiNhans { get; set; }
    }
    public class NguoiNhan
    {
        public int Id { get; set; }
        public string FullName { get; set; }
    }
}
