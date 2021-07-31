using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HumanResource.Data.Enum
{
    public enum ErrorCodeEnum
    {
        [Description("Thành công")]
        Success = 0,
        [Description("Thêm mới dữ liệu không thành công")]
        InsertError = 1,
        [Description("Lấy dữ liệu không thành công")]
        GetError = 2,
        [Description("Sửa dữ liệu không thành công")]
        PutError = 3,
        [Description("Xóa dữ liệu không thành công")]
        DeleteError = 4,
        [Description("Mã đã tồn tại trong hệ thống")]
        CodeExist = 5,
        [Description("Tài khoản chưa được xác thực")]
        TokenFail = 6,
        [Description("Lưu dữ liệu không thành công")]
        SaveFail = 7,
        [Description("Lỗi tài nguyên hệ thống")]
        Exception = 8,
    }
}
