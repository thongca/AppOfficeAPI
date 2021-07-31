using HumanResource.Data.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Response
{
   public class ResponseBase<T>
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
        public string Message { get; set; }
        public ErrorCodeEnum Code { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}
