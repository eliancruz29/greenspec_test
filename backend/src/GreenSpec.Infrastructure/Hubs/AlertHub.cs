using GreenSpec.Application.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace GreenSpec.Infrastructure.Hubs;

public class AlertHub : Hub
{
    public async Task BroadcastAlert(AlertDto alert)
    {
        await Clients.All.SendAsync("ReceiveAlert", alert);
    }
}
