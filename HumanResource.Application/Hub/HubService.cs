using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace HumanResource.Application.Hub
{
   public class HubService
    {
        public static HubService Instance { get; } = new HubService();
        private static HubConnection hubConnection;
        public async void Initialized()
        {
            try
            {
                hubConnection = new HubConnectionBuilder()
              .WithUrl("http://192.168.24.103:4646/signalrtc")
              .Build();
                await hubConnection.StartAsync();
            }
            catch (Exception)
            {
            }

            
        }
        public async void CallHub(NotifyContent notifyContent, string connectionId)
        {
            try
            {
                await hubConnection.InvokeAsync("SendData", notifyContent, connectionId);
            }
            catch (Exception)
            {
            }
          
        }
        public async void PushTypeFlow(int[] typeflows, string connectionId)
        {
            try
            {
                await hubConnection.InvokeAsync("PushNotifySign", typeflows, connectionId);
            }
            catch (Exception)
            {
            }

        }
    }
}
