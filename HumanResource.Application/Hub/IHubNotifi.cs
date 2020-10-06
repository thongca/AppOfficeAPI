using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HumanResource.Application.Hub
{
   public interface IHubNotifi
    {
        public Task PushNotifiHubAsync();

        public Task SendData(string signal);

    }
}
