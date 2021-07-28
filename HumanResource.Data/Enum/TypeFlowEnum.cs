using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HumanResource.Data.Enum
{
    public enum TypeFlowEnum
    {
        /// <summary>
        /// Chưa bắt đầu (mới tạo) 0
        /// </summary>
        [Description("Chưa bắt đầu (mới tạo)")]
        New = 0,
        /// <summary>
        /// Trình phê duyệt thời hạn 1
        /// </summary>
        [Description("Trình phê duyệt thời hạn")]
        TrinhPheDuyetThoiHan = 1,
        /// <summary>
        /// Đã phê duyệt thời hạn có chỉnh sửa 2
        /// </summary>
        [Description("Đã phê duyệt thời hạn có chỉnh sửa")]
        DaPheDuyetThoiHanCoChinhSua = 2,
        /// <summary>
        /// Đã phê duyệt thời hạn không chỉnh sửa 3
        /// </summary>
        [Description("Đã phê duyệt thời hạn không chỉnh sửa")]
        DaPheDuyetThoiHanKhongChinhSua = 3,
        /// <summary>
        /// Trình phê duyệt kết quả 4
        /// </summary>
        [Description("Trình phê duyệt kết quả")]
        TrinhPheDuyetKetQua = 4,
        /// <summary>
        /// Đã phê duyệt kết quả Yêu cầu chỉnh sửa 5
        /// </summary>
        [Description("Đã phê duyệt kết quả Yêu cầu chỉnh sửa")]
        DaPheDuyetKetQuaYeuCauChinhSua = 5,
        /// <summary>
        /// Đã phê duyệt kết quả đạt chất lượng 6
        /// </summary>
        [Description("Đã phê duyệt kết quả đạt chất lượng")]
        DaPheDuyetKetQuaDatChatLuong = 6,
        /// <summary>
        /// Chỉnh sửa phát sinh
        /// </summary>
        [Description("Chỉnh sửa phát sinh")]
        ChinhSuaPhatSinh = 7,
        /// <summary>
        /// Duyệt chỉnh sửa phát sinh
        /// </summary>
        [Description("Duyệt chỉnh sửa phát sinh")]
        DuyetChinhSuaPhatSinh = 8,
        /// <summary>
        /// Nhắc nhở gia hạn
        /// </summary>
        [Description("Nhắc nhở gia hạn")]
        NhacNhoGiaHan = 9,
        /// <summary>
        /// Bị đánh giá chất lượng công việc
        /// </summary>
        [Description("Bị đánh giá chất lượng công việc")]
        BiDanhGiaChatLuong = 10,
        /// <summary>
        /// Chuyển công việc cho người tiếp theo
        /// </summary>
        [Description("Chuyển công việc cho người tiếp theo")]
        ChuyenChoNguoiTiepTheo = 11,
        /// <summary>
        /// Chuyển kết quả cc cho người theo dõi
        /// </summary>
        [Description("Chuyển kết quả cc cho người theo dõi")]
        CCNguoiTheoDoi = 12,
        /// <summary>
        /// Công việc khởi tạo sau
        /// </summary>
        [Description("Công việc khởi tạo sau")]
        CongViecKhoiTaoSau = 13,
        /// <summary>
        /// Trình hoàn thành công việc khởi tạo sau
        /// </summary>
        [Description("Trình hoàn thành công việc khởi tạo sau")]
        TrinhHoanThanhKhoiTaoSau = 14,
        /// <summary>
        /// Không đồng ý cho phép khởi tạo công việc sau
        /// </summary>
        [Description("Không đồng ý cho phép khởi tạo công việc sau")]
        KhongDuyetKhoiTaoSau = 15,
        /// <summary>
        /// Khởi tạo sau được duyệt
        /// </summary>
        [Description("Khởi tạo sau được duyệt")]
        DuyetCongViecKhoiTaoSau = 16,
        /// <summary>
        /// Trình giải quyết phối hợp công tác
        /// </summary>
        [Description("Trình giải quyết phối hợp công tác")]
        TrinhGiaiQuyetPhoiHopCongTac = 17,
        /// <summary>
        /// Đã Xử lý phối hợp công tác
        /// </summary>
        [Description("Đã Xử lý phối hợp công tác")]
        DaXuLyPhoiHopCongTac = 17,

    }
}
