using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vehicles.Hubs
{
    public interface IChatClient
    {
        Task ReceiveMessage(string user, string message);
        Task SendMessageToCaller(string client, string message);
    }


    public class ChatHubStrongly: Hub<IChatClient>
    {
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string client, string message)
        {
            await Clients.All.ReceiveMessage(client, message);
        }

        public async Task SendMessageToCaller(string client, string message)
        {
            await Clients.Caller.ReceiveMessage(client, message);
        }
    }
    public class ChatHub : Hub
    {
        [HubMethodName("SendMessageToUser")]
        public Task DirectMessage(string user, string message)
        {
            return Clients.User(user).SendAsync("ReceiveMessage", user, message);
        }
        public Task SendMessage(string user, string message)               
        {
            return Clients.All.SendAsync("ReceiveMessage", user, message);    
        }

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupName}.");
        }
    }
}
