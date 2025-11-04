using GreenSpec.Application.DTOs;
using GreenSpec.Application.Interfaces;
using GreenSpec.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace GreenSpec.Infrastructure.Services;

public class AlertNotifier(IHubContext<AlertHub> hubContext) : IAlertNotifier
{
    public async Task NotifyAlertAsync(AlertDto alert, CancellationToken cancellationToken = default)
    {
        await hubContext.Clients.All.SendAsync("ReceiveAlert", alert, cancellationToken);
    }
}
