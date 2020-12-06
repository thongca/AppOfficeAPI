using HumanResource.Application.Hub;
using HumanResource.Data.EF;
using HumanResource.Data.Entities.System;
using HumanResoureAPI.Common;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
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
        public async Task SendActionToClient(int Id, string user)
        {
            var userInfo = new UserInfo() { UserId = Id, ConnectionId = Context.ConnectionId };
            await Clients.Client(user).SendAsync("SendActionToClient", JsonSerializer.Serialize(userInfo));

        }

        public string GetConnectionId(string userId)
        {

            string connectionId = Context.ConnectionId;
            var contact = _context.Sys_Dm_Connection.FirstOrDefault(x => x.UserId == Convert.ToInt32(userId));
            if (contact != null)
            {
                contact.ConnectionId = connectionId;
                contact.CreatedDate = TransforDate.FromDateToDouble(DateTime.Now);
            }
            else
            {
                Sys_Dm_Connection obj = new Sys_Dm_Connection()
                {
                    UserId = Convert.ToInt32(userId),
                    ConnectionId = connectionId,
                    CreatedDate = TransforDate.FromDateToDouble(DateTime.Now)
                };
                _context.Sys_Dm_Connection.Add(obj);
            }
            _context.SaveChanges();
            return connectionId;
        }
        public async Task SendData(NotifyContent notifyContent, string connectionId)
        {
            try
            {
                await Clients.Client(connectionId).SendAsync("SendSignal", notifyContent);
            }
            catch (Exception)
            {
            }

        }
        /// <summary>
        /// Đẩy thông báo duyệt đến cho người dùng client
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public async Task PushNotifySign(int[] signal, string connectionId)
        {
            try
            {
                await Clients.Client(connectionId).SendAsync("SenNotifySign", signal);
            }
            catch (Exception)
            {

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
