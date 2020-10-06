using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.VanBan
{
   public class VB_QT_VanBanMoiSoHoa
    {
        public string Id { get; set; }
        public string TrichYeu { get; set; }
        public int LoaiVanBanId { get; set; }
        public int CompanyId { get; set; }
        public int DepartmentId { get; set; }
        public string SoKyHieu { get; set; }
        public string NoiBanHanh { get; set; }
        public DateTime NgayBanHanh { get; set; }
        public int NguoiKyId { get; set; }
        public int ChucVuId { get; set; }
        public int LinhVucId { get; set; }
        public int SoTrang { get; set; }
        public string TuKhoa { get; set; }
        public string TenNguoiKy { get; set; }
        public int UserCreateId { get; set; }
        public DateTime CreateDate { get; set; }
        public string TenNguoiTao { get; set; }

    }
}
