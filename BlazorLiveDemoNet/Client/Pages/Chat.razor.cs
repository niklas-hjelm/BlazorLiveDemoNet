using BlazorLiveDemoNet.Shared.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace BlazorLiveDemoNet.Client.Pages;

partial class Chat : ComponentBase
{
    public ChatMessageDto CurrentMessage { get; set; } = new();
    public List<ChatMessageDto> Messages { get; set; }

    private HubConnection _chatHub;

    protected override async Task OnInitializedAsync()
    {
        Messages = new List<ChatMessageDto>();

        _chatHub = new HubConnectionBuilder()
            .WithUrl(NavigationManager.BaseUri + "hubs/chat")
            .Build();

        _chatHub.On<ChatMessageDto>("BroadcastMessage", (message) =>
        {
            Messages.Add(message);
            StateHasChanged();
        });

        await _chatHub.StartAsync();
        await base.OnInitializedAsync();
    }

    private async Task SendMessage()
    {
        CurrentMessage.Timestamp = DateTime.UtcNow;
        await _chatHub.SendAsync("BroadcastMessage", CurrentMessage);
        CurrentMessage.Message = string.Empty;
    }

}