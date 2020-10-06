using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HumanResource.Application.Mail;
using HumanResource.Data.EF;
using HumanResoureAPI.Common.Systems;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HumanResoureAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailerController : ControllerBase
    {
        private readonly IMailer _mailer;
        private readonly humanDbContext _context;
        public MailerController(humanDbContext context, IMailer mailer)
        {
            _context = context;
            _mailer = mailer;
        }

        [HttpGet]
        public async Task<IActionResult> WorkFlow()
        {
            try
            {
                await _mailer.SendMailAsync("ndthongit@gmail.com", "Quy trình dòng chảy công việc", "Trình phê duyệt thời hạn");
                return new JsonResult(new { error = 0, ms = "Gửi mail thành công!" });
            }
            catch (Exception e)
            {
                return new JsonResult(new { error = 1, ms = "Gửi mail không thành công!", e.Message });
            }
          
        }
    }
}