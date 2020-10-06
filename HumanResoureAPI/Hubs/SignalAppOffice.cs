using HumanResource.Application.Hub;
using HumanResource.Data.EF;
using HumanResoureAPI.Common;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace HumanResoureAPI.Hubs
{
    public class SignalAppOffice : Hub, IHubNotifi
    {
        private readonly humanDbContext _context;
        public SignalAppOffice(humanDbContext context)
        {
            _context = context;
        }
        //public async Task SendActionToClient(int Id, string user)
        //{
        //    var userInfo = new UserInfo() { UserId = Id, ConnectionId = Context.ConnectionId };
        //    await Clients.Client(user).SendAsync("SendActionToClient", JsonSerializer.Serialize(userInfo));
        //}

        public async Task SendData(string signal)
        {
            try
            {
                if (signal != "") // khi  truyển người nhận từ client xuống thì gửi cho những người đó
                {
                    await Clients.All.SendAsync("SendSignal", signal);

                }
                else // không thì gửi cho những người chưa nhận được thông báo isNotifi  ==  false
                {
                    var nguoiNhanTBs = _context.Sys_QT_ThongBao.Where(x => x.IsNotifi == false);
                    var listSend = await nguoiNhanTBs.Select(x => new { 
                    x.NguoiNhanId
                    }).ToListAsync();
                    string jsonNguoiNhans = JsonSerializer.Serialize(listSend);
                    await Clients.All.SendAsync("SendSignal", jsonNguoiNhans);
                    var listChange = await nguoiNhanTBs.ToListAsync(); // khi gửi xong thì chuyển isNotifi = true
                    foreach (var item in listChange)
                    {
                        item.IsNotifi = true;
                    }
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
           
        }

        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            await Clients.All.SendAsync("UserDisconnect", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public Task PushNotifiHubAsync()
        {
            throw new NotImplementedException();
        }
    }
}
