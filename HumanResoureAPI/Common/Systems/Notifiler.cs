using HumanResource.Application.Hub;
using HumanResource.Application.Notifi;
using HumanResource.Application.Paremeters.Dtos;
using HumanResource.Data.Request;using HumanResource.Data.EF;
using HumanResource.Data.Entities.System;
using HumanResource.Data.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace HumanResoureAPI.Common.Systems
{
    public class Notifiler : INotifi
    {
        private readonly humanDbContext _context;
        private readonly IHubNotifi _hubnoti;
        public Notifiler(humanDbContext context, IHubNotifi hubnoti)
        {
            _context = context;
            _hubnoti = hubnoti;
            HubService.Instance.Initialized();
        }
        public async Task SaveNotifiAsync(int QuyTrinhId, string MaLenh, string TenNguoiGui, string NoiDung, int NguoiNhanId, TypeFlowEnum TrangThaixl, string router)
        {

            Sys_QT_ThongBao sys_QT_ThongBao = new Sys_QT_ThongBao()
            {
                QuyTrinhId = QuyTrinhId,
                MaLenh = MaLenh,
                TenNguoiGui = TenNguoiGui,
                NoiDung = NoiDung,
                NgayGui = DateTime.Now,
                NgayDoc = null,
                DaDoc = false,
                RouterLink = router,
                NguoiNhanId = NguoiNhanId,
                TrangThaiXuLy = (int) TrangThaixl,
                TrangThai = getTrangThaiXuLy((int)TrangThaixl, QuyTrinhId),
                IsNotifi = true
            };
            NotifyContent notifyContent = new NotifyContent()
            {
                TenNguoiGui = sys_QT_ThongBao.TenNguoiGui,
                TrangThai = sys_QT_ThongBao.TrangThai,
                Ngay = DateTime.Now,
                NoiDung = sys_QT_ThongBao.NoiDung

            };
            var connect = await _context.Sys_Dm_Connection.FirstOrDefaultAsync(x => x.UserId == NguoiNhanId);
            if (connect != null)
            {
                int[] typeflow = { 2, 3, 5, 6, 16, 15 };
                if (QuyTrinhId == 2 && typeflow.Contains((int)TrangThaixl))
                {
                    int[] pushtypef = {(int) TrangThaixl };
                    HubService.Instance.PushTypeFlow(pushtypef, connect.ConnectionId);
                }
                HubService.Instance.CallHub(notifyContent, connect.ConnectionId);
            }
            else
            {
                sys_QT_ThongBao.IsNotifi = false;
            }
            _context.Sys_QT_ThongBao.Add(sys_QT_ThongBao);
            await _context.SaveChangesAsync();


        }
        // đẩy thông báo cho người có công đoạn tiếp theo biết công việc tiên quyết đã hoàn thành
        public async Task SaveNotifiNextAsync(int? Code, int QuyTrinhId, string MaLenh, string TenNguoiGui, string NoiDung, TypeFlowEnum TrangThaixl, string router)
        {
            try
            {
                var myWorks = _context.CV_QT_MyWork.Where(x => x.Predecessor == Code).Select(a => a.UserTaskId);
                string messege = "(CV Tiên quyết của mã công việc (" + Code.ToString() + ") đã hoàn thành";
                foreach (var item in myWorks)
                {
                    Sys_QT_ThongBao sys_QT_ThongBao = new Sys_QT_ThongBao()
                    {
                        QuyTrinhId = QuyTrinhId,
                        MaLenh = MaLenh,
                        TenNguoiGui = TenNguoiGui,
                        NoiDung = messege,
                        NgayGui = DateTime.Now,
                        NgayDoc = null,
                        DaDoc = false,
                        RouterLink = router,
                        NguoiNhanId = item,
                        TrangThaiXuLy = (int) TrangThaixl,
                        TrangThai = getTrangThaiXuLy((int)TrangThaixl, QuyTrinhId),
                        IsNotifi = false
                    };
                    _context.Sys_QT_ThongBao.Add(sys_QT_ThongBao);
                }
                await _context.SaveChangesAsync();

            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }

        }
        private static string getTrangThaiXuLy(int ttxl, int QuytrinhId)
        {
            if (QuytrinhId == 1)
            {
                switch (ttxl)
                {
                    case 1:
                        return "Văn bản mới số hóa";
                    case 2:
                        return "Văn bản chờ phê duyệt";
                    case 3:
                        return "Văn bản đã phê duyệt";
                    case 4:
                        return "Văn bản bị trả lại";
                    case 5:
                        return "Văn bản nhận thông báo";
                    case 6:
                        return "Văn bản xử lý chính";
                    case 7:
                        return "Văn bản đồng xử lý";
                    case 8:
                        return "Văn bản nhận để biết";
                    default:
                        return "Văn bản mới số hóa";
                }
            }
            else
            {
                switch (ttxl)
                {
                    case 1:
                        return "Trình phê duyệt thời hạn";
                    case 2:
                        return "Đã phê duyệt thời hạn có chỉnh sửa";
                    case 3:
                        return "Đã phê duyệt thời hạn";
                    case 4:
                        return "Trình phê duyệt kết quả";
                    case 5:
                        return "Yêu cầu chỉnh sửa";
                    case 6:
                        return "Đã phê duyệt đạt chất lượng";
                    case 7:
                        return "Chỉnh sửa phát sinh";
                    case 8:
                        return "Duyệt chỉnh sửa phát sinh";
                    case 9:
                        return "Bị đánh giá CLCV";
                    case 10:
                        return "Nhắc nhở và gia hạn";
                    case 11:
                        return "Chuyển công việc";
                    case 12:
                        return "Chuyển kết quả cc";
                    case 14:
                        return "Trình công việc khởi tạo sau";
                    case 15:
                        return "Không đồng ý khởi tạo sau";
                    case 16:
                        return "Khởi tạo sau được duyệt";
                    case 17:
                        return "Trình phối hợp công tác";
                    case 18:
                        return "Chuyển tiếp công việc";
                    default:
                        return "Công việc mới";
                }
            }

        }


    }
}
