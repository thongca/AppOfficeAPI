using HumanResource.Application.Paremeters.Dtos;
using HumanResource.Data.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HumanResource.Application.Notifi
{
   public interface INotifi
    {
        // lưu thông báo cho người nhận thực hiện công việc tiếp theo
        public Task SaveNotifiNextAsync(int? Code, int QuyTrinhId, string MaLenh, string TenNguoiGui, string NoiDung, TypeFlowEnum TrangThaixl, string router);
        public Task SaveNotifiAsync(int QuyTrinhId, string MaLenh, string TenNguoiGui, string NoiDung, int NguoiNhanId, TypeFlowEnum TrangThai, string router);
    }
}
