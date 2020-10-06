using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HumanResource.Application.Mail
{
   public interface IMailer
    {
        public Task SendMailAsync(string email, string subject, string body);
    }
}
