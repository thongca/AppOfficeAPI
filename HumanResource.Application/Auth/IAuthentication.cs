using System;
using System.Collections.Generic;
using System.Text;
using HumanResource.Application.Helper.Dtos;

namespace HumanResource.Application.Helper
{
   public interface IAuthentication
    {
        public string GenerateToken(string claimTypes, RequestToken request);
    }
}
