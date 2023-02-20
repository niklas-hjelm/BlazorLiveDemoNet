using BlazorLiveDemoNet.Shared.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace BlazorLiveDemoNet.Server.Hubs;

public class ChatHub : Hub
{
    public async Task BroadcastMessage(ChatMessageDto message)
    {
        await Clients.All.SendAsync("BroadcastMessage", message);
    }
}