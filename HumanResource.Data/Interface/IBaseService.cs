using HumanResource.Data.Enum;
using HumanResource.Data.Request;
using HumanResource.Data.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Interface
{
   public interface IBaseService <T, TRequest, TResponse>
    {
        public ErrorCodeEnum CreateItem(TRequest request, RequestToken token);
        public ErrorCodeEnum EditItem(TRequest request, RequestToken token);
        public ErrorCodeEnum DeletedItem(int? Id, RequestToken token);
        public ErrorCodeEnum DeletedItems(List<int?> Ids, RequestToken token);
        public TResponse GetItem(int Id, RequestToken token);
        public ResponseBase<TResponse> GetItems(TRequest request, RequestToken token);
        public T DataDefault(T data, RequestToken token);
        /// <summary>
        /// Kiểm tra tồn tại của mã
        /// </summary>
        /// <param name="request"></param>
        /// <returns>true = đã tồn tại, false = chưa tồn tại</returns>
        public bool CheckExistCode(T request);
    }
}
